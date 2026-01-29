namespace AssesmentUC.Infrastructure.Data
{
    /// <summary>
    /// Nombres de stored procedures utilizados en la aplicaci�n.
    /// Centralizar estos nombres evita errores tipogr�ficos y facilita mantenimiento.
    /// </summary>
    public static class StoredProcedureNames
    {
        // ===== ENCUESTAS =====
        public const string SP_LISTAR_PLANTILLA_ENCUESTAS = "ENC.sp_ListarPlantillaEncuestas_UC";
        public const string SP_LISTAR_ASIGNATURA_ENCUESTAS = "ENC.sp_ListarAsignaturaEncuestas_UC";
        public const string SP_LISTAR_PLANTILLA_ENCUESTA_ID = "ENC.sp_ListarPlantillaEncuestaId";
        public const string SP_LISTAR_TIPO_ENCUESTA = "ENC.sp_ListarTipoEncuesta_UC";
        public const string SP_LISTAR_TIPO_ENCUESTADO = "ENC.sp_ListarTipoEncuestado";
        public const string SP_LISTAR_SEDES = "BANINST1.SZKENCU.P_LISTAR_SEDES";
        public const string SP_LISTAR_PERIODOS = "BANINST1.SZKENCU.P_LISTAR_PERIODOS";
        public const string SP_LISTAR_SECCIONES = "BANINST1.SZKENCU.P_LISTAR_SECCIONES";
        public const string SP_LISTAR_ASIGNATURAS = "BANINST1.SZKENCU.P_LISTAR_ASIGNATURAS";
        public const string SP_LISTAR_DOCENTES = "BANINST1.SZKENCU.P_LISTAR_DOCENTES_ASIGNATURA";
        public const string SP_LISTAR_TIPO_PROGRAMA = "BANINST1.SZKENCU.P_LISTAR_TIPO_PROGRAMA";
        public const string SP_LISTAR_ASESORES = "BANINST1.SZKENCU.P_LISTAR_ASESORES";
        public const string SP_CREAR_ENCUESTA_ASIGNATURA = "ENC.sp_CrearEncuestaAsignatura_UC";
        public const string SP_CREAR_ENCUESTA_PLANTILLA = "ENC.sp_CrearEncuestaPlantilla_UC";
        public const string SP_EDITAR_PLANTILLA_ENCUESTA = "ENC.sp_EditarPlantillaEncuesta_UC";
        public const string SP_ELIMINAR_ENCUESTA_PLANTILLA = "ENC.sp_EliminarEncuestaPlantilla_UC";
        public const string SP_ELIMINAR_BLOQUE_PLANTILLA = "ENC.sp_EliminarBloquePlantilla_UC";
        public const string SP_ELIMINAR_PREGUNTA_PLANTILLA = "ENC.sp_EliminarPreguntaPlantilla_UC";
        public const string SP_LISTAR_CORREOS_ENCUESTA = "BANINST1.SZKENCU.P_LISTAR_CORREOS_ENCUESTA";

        // ===== RESPUESTAS =====
        public const string SP_LISTAR_ENCUESTAS_RESPONDIDAS = "ENC.sp_ListarRencuestasRespondidas_UC";
        public const string SP_LISTAR_PREGUNTAS_ENCUESTA = "ENC.sp_ListarAsignaturaEncuestaId_UC";
        public const string SP_LISTAR_ENCUESTAS_PENDIENTES = "ENC.sp_ListarEncuestasPendientes_UC";
        public const string SP_CREAR_RESPUESTA_ENCUESTA = "ENC.sp_CrearRespuestaEncuesta_UC";
        public const string SP_CREAR_RESPUESTA_PREGUNTA = "ENC.sp_CrearRespuestaPregunta_UC";
        public const string SP_ACTUALIZAR_ENCUESTA_ENVIADA = "ENC.sp_ActualizarEncuestaEnviada_UC";
        public const string SP_VALIDAR_RESPUESTA_ALUMNO = "ENC.sp_ValidarRespuestaAlumno_UC";
        public const string SP_VALIDAR_ENCUESTA_ACTIVA = "ENC.sp_ValidarEncuestaActiva_UC";
        public const string SP_NOMBRE_DOCENTE_DNI = "BANINST1.SZKENCU.P_NOMBRE_DOCENTE_DNI";

        // ===== REPORTES =====
        public const string SP_LISTAR_REPORTE_ENCUESTA_ALUMNO = "ENC.sp_ListarReporteEncuestaAlumno_UC";
        public const string SP_LISTAR_REPORTE_ENCUESTA_DOCENTE = "ENC.sp_ListarReporteEncuestaDocente_UC";
        public const string SP_LISTAR_REPORTE_ENCUESTA_ASESOR = "ENC.sp_ListarReporteEncuestaAsesor_UC";
        public const string SP_LISTAR_REPORTE_PARA_EXCEL = "ENC.sp_ListarReporteParaExcel_UC";
    }
}
