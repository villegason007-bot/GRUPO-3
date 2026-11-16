#!/usr/bin/env python3
"""Sembrar la comisión 2K1 con 50 estudiantes y sus relaciones académicas."""

import sqlite3
import sys
import uuid
from pathlib import Path

DB = (
	Path(sys.argv[1])
	if len(sys.argv) > 1
	else Path(__file__).resolve().parents[2] / "Gestor de Estudiantes" / "data" / "ppii.db"
)

COMISION = "2K1"
TOTAL = 50
FECHA = "%s 00:00:00"

ALUMNOS = [
	("Aldana", "Molina"), ("Benjamín", "Paz"), ("Carolina", "Gómez"), ("Diego", "Sánchez"), ("Elena", "Torres"),
	("Fabián", "Luna"), ("Gabriela", "Rojas"), ("Hernán", "Méndez"), ("Iara", "Castro"), ("Juan", "Pereyra"),
	("Karen", "Silva"), ("Leandro", "Vega"), ("Micaela", "Suárez"), ("Nahuel", "Acosta"), ("Ornella", "Navarro"),
	("Pablo", "Quiroga"), ("Rocío", "Bustos"), ("Santiago", "Ferreyra"), ("Tamara", "Ledesma"), ("Ulises", "Ibarra"),
	("Valentina", "Ruiz"), ("Walter", "Vargas"), ("Ximena", "Correa"), ("Yanina", "Miranda"), ("Zoe", "Benítez"),
	("Agustín", "Cardozo"), ("Bianca", "Salinas"), ("Ciro", "Aguirre"), ("Daiana", "Cabrera"), ("Ezequiel", "Ocampo"),
	("Florencia", "Peralta"), ("Gonzalo", "Ayala"), ("Helena", "Medina"), ("Iván", "Bravo"), ("Jimena", "Sosa"),
	("Kevin", "Núñez"), ("Luciana", "Ríos"), ("Matías", "Marín"), ("Noelia", "Villalba"), ("Octavio", "Espínola"),
	("Paula", "Romero"), ("Ramiro", "Ibañez"), ("Selene", "Quezada"), ("Tomás", "Villegas"), ("Úrsula", "Montenegro"),
	("Victoria", "Acuña"), ("William", "Domínguez"), ("Xavier", "Rey"), ("Yamila", "Moreno"), ("Bruno", "Ponce"),
]

MATERIAS = [
	"Programación II",
	"Bases de Datos",
	"Ingeniería de Software",
	"Prácticas Profesionalizantes",
]

CLASES = {
	"Programación II": ["2026-08-10", "2026-08-17", "2026-08-24"],
	"Bases de Datos": ["2026-08-11", "2026-08-18", "2026-08-25"],
	"Ingeniería de Software": ["2026-08-12", "2026-08-19", "2026-08-26"],
	"Prácticas Profesionalizantes": ["2026-08-13", "2026-08-20", "2026-08-27"],
}

TRABAJOS = {
	"Programación II": [("Programación II - TP1", "2026-08-18"), ("Programación II - TP2", "2026-08-31")],
	"Bases de Datos": [("Bases de Datos - TP1", "2026-08-19"), ("Bases de Datos - TP2", "2026-09-01")],
	"Ingeniería de Software": [("Ingeniería de Software - TP1", "2026-08-20"), ("Ingeniería de Software - TP2", "2026-09-02")],
	"Prácticas Profesionalizantes": [
		("Prácticas Profesionalizantes - TP1", "2026-08-21"),
		("Prácticas Profesionalizantes - TP2", "2026-09-03"),
	],
}


def uid() -> str:
	return str(uuid.uuid4())


def main() -> None:
	con = sqlite3.connect(DB)
	con.execute("PRAGMA foreign_keys = ON")

	try:
		con.execute("BEGIN")

		fila = con.execute("SELECT id FROM comisiones WHERE codigo = ? COLLATE NOCASE", (COMISION,)).fetchone()
		comision_id = fila[0] if fila else uid()
		if fila is None:
			con.execute(
				"INSERT INTO comisiones (id, codigo, semanas) VALUES (?, ?, 16)",
				(comision_id, COMISION),
			)

		materia_ids = {}
		for nombre in MATERIAS:
			m = con.execute("SELECT id FROM materias WHERE nombre = ? COLLATE NOCASE", (nombre,)).fetchone()
			materia_id = m[0] if m else uid()
			if m is None:
				con.execute("INSERT INTO materias (id, nombre) VALUES (?, ?)", (materia_id, nombre))
			con.execute(
				"INSERT OR IGNORE INTO materia_comision (materia_id, comision_id) VALUES (?, ?)",
				(materia_id, comision_id),
			)
			materia_ids[nombre] = materia_id

		for i in range(1, TOTAL + 1):
			legajo = f"500{i:02d}"
			nombre, apellido = ALUMNOS[i - 1]
			telefono = f"11-{4100 + i}-{5200 + i}"
			fecha_incorporacion = f"2026-07-{(i % 28) + 1:02d}"
			est = con.execute("SELECT id FROM estudiantes WHERE legajo = ?", (legajo,)).fetchone()
			if est is None:
				estudiante_id = uid()
				con.execute(
					"""INSERT INTO estudiantes
					   (id, legajo, nombre, apellido, telefono, fecha_incorporacion, activo, comision_id)
					   VALUES (?, ?, ?, ?, ?, ?, 1, ?)""",
					(
						estudiante_id,
						legajo,
						nombre,
						apellido,
						telefono,
						fecha_incorporacion,
						comision_id,
					),
				)
			else:
				estudiante_id = est[0]
				con.execute(
					"""UPDATE estudiantes
					   SET nombre = ?, apellido = ?, telefono = ?, fecha_incorporacion = ?, activo = 1, comision_id = ?
					   WHERE id = ?""",
					(nombre, apellido, telefono, fecha_incorporacion, comision_id, estudiante_id),
				)

			for materia_id in materia_ids.values():
				con.execute(
					"INSERT OR IGNORE INTO estudiante_materia (estudiante_id, materia_id) VALUES (?, ?)",
					(estudiante_id, materia_id),
				)

		for idx_materia, nombre in enumerate(MATERIAS):
			materia_id = materia_ids[nombre]

			for idx_clase, fecha in enumerate(CLASES[nombre]):
				estado = "NoHabil" if nombre == "Prácticas Profesionalizantes" and idx_clase == 2 else "Habil"
				c = con.execute(
					"SELECT id FROM clases WHERE comision_id = ? AND materia_id = ? AND fecha = ?",
					(comision_id, materia_id, FECHA % fecha),
				).fetchone()
				if c is None:
					con.execute(
						"INSERT INTO clases (id, comision_id, materia_id, fecha, estado) VALUES (?, ?, ?, ?, ?)",
						(uid(), comision_id, materia_id, FECHA % fecha, estado),
					)

			for titulo, fecha in TRABAJOS[nombre]:
				t = con.execute(
					"SELECT id FROM trabajos WHERE comision_id = ? AND materia_id = ? AND titulo = ?",
					(comision_id, materia_id, titulo),
				).fetchone()
				if t is None:
					con.execute(
						"INSERT INTO trabajos (id, comision_id, materia_id, titulo, fecha_entrega) VALUES (?, ?, ?, ?, ?)",
						(uid(), comision_id, materia_id, titulo, FECHA % fecha),
					)

			clases_habiles = con.execute(
				"SELECT id, fecha FROM clases WHERE comision_id = ? AND materia_id = ? AND estado = 'Habil'",
				(comision_id, materia_id),
			).fetchall()
			trabajos = con.execute(
				"SELECT id, titulo FROM trabajos WHERE comision_id = ? AND materia_id = ?",
				(comision_id, materia_id),
			).fetchall()
			estudiantes = con.execute(
				"SELECT id, legajo FROM estudiantes WHERE comision_id = ? AND activo = 1 ORDER BY legajo",
				(comision_id,),
			).fetchall()

			for est_id, legajo in estudiantes:
				n = int(legajo[-2:])
				for clase_id, fecha in clases_habiles:
					dia = int(fecha[8:10])
					cond = "Ausente" if (n + idx_materia + dia) % 5 == 0 else "Presente"
					con.execute(
						"INSERT OR IGNORE INTO asistencias (id, clase_id, estudiante_id, condicion) VALUES (?, ?, ?, ?)",
						(uid(), clase_id, est_id, cond),
					)
				for trabajo_id, titulo in trabajos:
					ok = (n + idx_materia + len(titulo)) % 4 != 0
					con.execute(
						"INSERT OR IGNORE INTO entregas (id, trabajo_id, estudiante_id, entregado, fecha) VALUES (?, ?, ?, ?, ?)",
						(uid(), trabajo_id, est_id, 1 if ok else 0, "2026-08-25 12:00:00" if ok else None),
					)

		con.commit()

		total = con.execute(
			"SELECT COUNT(*) FROM estudiantes WHERE comision_id = ? AND activo = 1",
			(comision_id,),
		).fetchone()[0]
		print(f"Comisión {COMISION} lista con {total} estudiantes activos.")
	except Exception:
		con.rollback()
		raise
	finally:
		con.close()


if __name__ == "__main__":
	main()
