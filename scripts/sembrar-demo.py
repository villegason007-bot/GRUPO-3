#!/usr/bin/env python3
"""Sembrar la base de datos demo del Gestor de Estudiantes (comisión 1K1).

Uso:
    python3 scripts/sembrar-demo.py [ruta/al/ppii.db]

Por defecto apunta a app/Gestor de Estudiantes/data/ppii.db. La base debe
existir con el esquema ya creado (arrancar una vez la consola, que llama a
BaseDeDatos.Inicializar()). Genera: 30 estudiantes, 9 clases (8 hábiles y
1 no hábil), 4 trabajos (3 vencidos + 1 futuro), asistencias y entregas con
un patrón determinista pensado para el demo del panel de riesgo (H8):
varios alumnos en riesgo, la mayoría sin riesgo, y una clase no hábil que
no debe contar en el cálculo.

La copia que viaja en el repo es docs/demo/ppii-demo.db; para usarla en
una máquina, copiarla sobre app/Gestor de Estudiantes/data/ppii.db.
"""
import sqlite3
import sys
import uuid
from pathlib import Path

DB = (
    Path(sys.argv[1])
    if len(sys.argv) > 1
    else Path(__file__).resolve().parents[1] / "app/Gestor de Estudiantes" / "data" / "ppii.db"
)

COMISION = "1K1"
FECHA = "%s 00:00:00"

NOMBRES = [
    ("Facundo", "Giménez"), ("Lucía", "Ferreyra"), ("Martín", "Ocampo"),
    ("Valentina", "Ríos"), ("Nicolás", "Benítez"), ("Camila", "Sosa"),
    ("Joaquín", "Ayala"), ("Martina", "Cabrera"), ("Franco", "Medina"),
    ("Julieta", "Peralta"), ("Tomás", "Ledesma"), ("Candelaria", "Ruiz"),
    ("Bautista", "Acosta"), ("Delfina", "Cardozo"), ("Lautaro", "Vera"),
    ("Emilia", "Nievas"), ("Gonzalo", "Ibarra"), ("Renata", "Bustos"),
    ("Bruno", "Salinas"), ("Malena", "Quinteros"), ("Agustín", "Marín"),
    ("Olivia", "Villalba"), ("Simón", "Aguirre"), ("Josefina", "Miranda"),
    ("Mateo", "Correa"), ("Paulina", "Espínola"), ("Ignacio", "Sarmiento"),
    ("Julián", "Bravo"), ("Abril", "Montenegro"), ("Thiago", "Núñez"),
]

CLASES_NO_HABILES = ["2026-08-01"]
CLASES_HABILES = [
    "2026-08-08", "2026-08-15", "2026-08-22", "2026-08-29",
    "2026-09-05", "2026-09-12", "2026-09-19", "2026-09-26",
]

TRABAJOS_VENCIDOS = [
    ("TP0 - Introducción", "2026-08-15"),
    ("TP1 - Modelo de dominio", "2026-08-30"),
    ("TP2 - Criterios de riesgo", "2026-09-20"),
]
TRABAJOS_FUTUROS = [("TP3 - Cierre", "2026-11-07")]

# Alumnos con muchas faltas (4 + i % 4) y los que no entregan trabajos.
RIESGO_POR_FALTAS = {3, 7, 11, 14, 18, 22, 25, 29}
SIN_ASISTENCIAS = {16, 27}
NO_ENTREGA_NADA = {3, 7, 11}          # 3 vencidos sin entregar (supera el límite 2)
NO_ENTREGA_DOS = {14, 18, 22}         # 2 sin entregar
NO_ENTREGA_UNO = {25, 29}             # 1 sin entregar

MAX_FALTAS, MAX_NO_ENTREGADOS, UMBRAL = 3, 2, 50


def main() -> None:
    con = sqlite3.connect(DB)
    con.execute("PRAGMA foreign_keys = ON")

    comision_id = con.execute(
        "SELECT id FROM comisiones WHERE codigo = ?", (COMISION,)
    ).fetchone()
    if comision_id is None:
        sys.exit(f"No existe la comisión {COMISION}. Arrancá la consola una vez para inicializar la base.")
    comision_id = comision_id[0]

    existentes = con.execute(
        "SELECT COUNT(*) FROM estudiantes WHERE comision_id = ?", (comision_id,)
    ).fetchone()[0]
    if existentes > 0:
        sys.exit(f"La comisión {COMISION} ya tiene {existentes} estudiantes; no se siembra dos veces.")

    # --- estudiantes ---
    alumnos = []
    for i, (nombre, apellido) in enumerate(NOMBRES, start=1):
        legajo = f"400{i:02d}"
        telefono = f"11-{2600 + i}-{3400 + i}" if i % 5 in (0, 1, 3) else None
        incorporacion = f"2026-03-{2 + (i % 10):02d}"
        con.execute(
            """INSERT INTO estudiantes
               (id, legajo, nombre, apellido, telefono, fecha_incorporacion, activo, comision_id)
               VALUES (?, ?, ?, ?, ?, ?, 1, ?)""",
            (str(uuid.uuid4()), legajo, nombre, apellido, telefono, incorporacion, comision_id),
        )
        alumnos.append((i, legajo, nombre, apellido))

    # --- clases (1 no hábil + 8 hábiles) ---
    clases = []
    for fecha in CLASES_NO_HABILES:
        eid = str(uuid.uuid4())
        con.execute(
            "INSERT INTO clases (id, comision_id, fecha, estado) VALUES (?, ?, ?, 'NoHabil')",
            (eid, comision_id, FECHA % fecha),
        )
    for fecha in CLASES_HABILES:
        eid = str(uuid.uuid4())
        con.execute(
            "INSERT INTO clases (id, comision_id, fecha, estado) VALUES (?, ?, ?, 'Habil')",
            (eid, comision_id, FECHA % fecha),
        )
        clases.append(eid)

    # --- trabajos: 3 vencidos + 1 futuro ---
    trabajos = []
    for titulo, fecha in TRABAJOS_VENCIDOS:
        eid = str(uuid.uuid4())
        con.execute(
            "INSERT INTO trabajos (id, comision_id, titulo, fecha_entrega) VALUES (?, ?, ?, ?)",
            (eid, comision_id, titulo, FECHA % fecha),
        )
        trabajos.append(eid)
    for titulo, fecha in TRABAJOS_FUTUROS:
        con.execute(
            "INSERT INTO trabajos (id, comision_id, titulo, fecha_entrega) VALUES (?, ?, ?, ?)",
            (str(uuid.uuid4()), comision_id, titulo, FECHA % fecha),
        )

    # --- asistencias (solo clases hábiles) ---
    for i, legajo, _, _ in alumnos:
        if i in SIN_ASISTENCIAS:
            continue
        estudiante_id = con.execute(
            "SELECT id FROM estudiantes WHERE legajo = ?", (legajo,)
        ).fetchone()[0]
        if i in RIESGO_POR_FALTAS:
            faltas = 4 + (i % 4)
        else:
            faltas = i % 3
        for j, clase_id in enumerate(clases):
            condicion = "Ausente" if j < faltas else "Presente"
            con.execute(
                "INSERT INTO asistencias (id, clase_id, estudiante_id, condicion) VALUES (?, ?, ?, ?)",
                (str(uuid.uuid4()), clase_id, estudiante_id, condicion),
            )

    # --- entregas de los vencidos (el futuro no lleva filas) ---
    for i, legajo, _, _ in alumnos:
        estudiante_id = con.execute(
            "SELECT id FROM estudiantes WHERE legajo = ?", (legajo,)
        ).fetchone()[0]
        if i in NO_ENTREGA_NADA:
            entregados = set()
        elif i in NO_ENTREGA_DOS:
            entregados = set(trabajos[:1])          # entregó solo TP0
        elif i in NO_ENTREGA_UNO:
            entregados = set(trabajos[:2])          # entregó TP0 y TP1
        else:
            entregados = set(trabajos)
        for trabajo_id in trabajos:
            con.execute(
                "INSERT INTO entregas (id, trabajo_id, estudiante_id, entregado, fecha) VALUES (?, ?, ?, ?, ?)",
                (
                    str(uuid.uuid4()),
                    trabajo_id,
                    estudiante_id,
                    1 if trabajo_id in entregados else 0,
                    "2026-09-18 12:00:00" if trabajo_id in entregados else None,
                ),
            )

    con.commit()
    imprimir_panel_de_control(con)
    con.close()


def imprimir_panel_de_control(con: sqlite3.Connection) -> None:
    """Replica las reglas del modelo (límites 3/2 y umbral 50) para verificar el demo."""
    filas = con.execute(
        """SELECT e.legajo, e.nombre, e.apellido,
                  (SELECT COUNT(*) FROM asistencias a
                   JOIN clases c ON c.id = a.clase_id
                   WHERE a.estudiante_id = e.id AND c.estado = 'Habil') AS con_registro,
                  (SELECT COUNT(*) FROM asistencias a
                   JOIN clases c ON c.id = a.clase_id
                   WHERE a.estudiante_id = e.id AND c.estado = 'Habil' AND a.condicion = 'Ausente') AS faltas,
                  (SELECT COUNT(*) FROM entregas en WHERE en.estudiante_id = e.id AND en.entregado = 1) AS entregados
           FROM estudiantes e WHERE e.activo = 1 ORDER BY e.legajo""",
    ).fetchall()

    clases_habiles = len(CLASES_HABILES)
    vencidos = len(TRABAJOS_VENCIDOS)
    en_riesgo, sin_riesgo = [], []
    for legajo, nombre, apellido, con_registro, faltas, entregados in filas:
        no_entregados = vencidos - entregados
        componentes, suma = 0, 0.0
        if con_registro and clases_habiles > 0:
            suma += faltas / clases_habiles * 100
            componentes += 1
        if vencidos > 0:
            suma += no_entregados / vencidos * 100
            componentes += 1
        puntaje = round(suma / componentes) if componentes else None
        riesgo = (
            puntaje is not None
            and ((con_registro > 0 and faltas > MAX_FALTAS)
                 or no_entregados > MAX_NO_ENTREGADOS
                 or puntaje >= UMBRAL)
        )
        motivo = f"faltas {faltas}/{clases_habiles} · no entregados {no_entregados}/{vencidos} · puntaje {puntaje}"
        if riesgo:
            en_riesgo.append((legajo, nombre, apellido, motivo))
        else:
            sin_riesgo.append((legajo, nombre, apellido, motivo))

    print(f"\n== Panel de control (max faltas {MAX_FALTAS}, max no entregados {MAX_NO_ENTREGADOS}, umbral {UMBRAL}) ==")
    print(f"Estudiantes: {len(filas)} · En riesgo: {len(en_riesgo)} · Sin riesgo: {len(sin_riesgo)}")
    print("-- En riesgo --")
    for legajo, nombre, apellido, motivo in en_riesgo:
        print(f"  {legajo} {nombre} {apellido}: {motivo}")
    print("-- Sin riesgo (primeros 5) --")
    for legajo, nombre, apellido, motivo in sin_riesgo[:5]:
        print(f"  {legajo} {nombre} {apellido}: {motivo}")


if __name__ == "__main__":
    main()
