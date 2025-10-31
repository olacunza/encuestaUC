
--USE --NOMBRENUEVABD
GO

DROP PROCEDURE ENCUESTA.sp_CrearEncuestaAsignatura

DROP PROCEDURE ENCUESTA.sp_CrearBloqueAsignatura

DROP PROCEDURE ENCUESTA.sp_CrearPreguntaAsignatura

DROP PROCEDURE ENCUESTA.sp_CrearEncuestaPlantilla

DROP PROCEDURE ENCUESTA.sp_CrearBloquePlantilla

DROP PROCEDURE ENCUESTA.sp_CrearPreguntaPlantilla

DROP PROCEDURE ENCUESTA.sp_ListarPlantillaEncuestas

DROP PROCEDURE ENCUESTA.sp_ListarAsignaturaEncuestas

DROP PROCEDURE ENCUESTA.sp_ListarTipoEncuesta

DROP PROCEDURE ENCUESTA.sp_ListarTipoEncuestado

DROP PROCEDURE ENCUESTA.sp_ListarPlantillaEncuestaId

DROP PROCEDURE ENCUESTA.sp_ListarAsignaturaEncuestaId

DROP PROCEDURE ENCUESTA.sp_ListarEncuestasPendientes

DROP PROCEDURE ENCUESTA.sp_EditarPlantillaEncuesta

DROP PROCEDURE ENCUESTA.sp_EditarBloquePlantilla

DROP PROCEDURE ENCUESTA.sp_EditarPreguntaPlantilla

DROP PROCEDURE ENCUESTA.sp_EliminarEncuestaPlantilla

DROP PROCEDURE ENCUESTA.sp_EliminarBloquePlantilla

DROP PROCEDURE ENCUESTA.sp_EliminarPreguntaPlantilla

DROP PROCEDURE ENCUESTA.sp_CrearRespuestaEncuesta

DROP PROCEDURE ENCUESTA.sp_CrearRespuestaPregunta

DROP PROCEDURE ENCUESTA.sp_ValidarRespuestaAlumno

DROP PROCEDURE ENCUESTA.sp_ValidarEncuestaActiva

DROP PROCEDURE ENCUESTA.sp_ListarRencuestasRespondidas

DROP PROCEDURE ENCUESTA.sp_ActualizarEncuestaEnviada

DROP PROCEDURE ENCUESTA.sp_ListarReporteEncuestaDocente

DROP PROCEDURE ENCUESTA.sp_ListarReporteEncuestaAlumno

DROP PROCEDURE ENCUESTA.sp_ListarReporteEncuestaAsesor

DROP PROCEDURE ENCUESTA.sp_ListarReporteParaExcel

DROP TABLE ENCUESTA.tblRespuetaPregunta

DROP TABLE ENCUESTA.tblRespuetaEncuesta

DROP TABLE ENCUESTA.tblEncuestaPreguntaAsignatura

DROP TABLE ENCUESTA.tblEncuestaPreguntaPlantilla

DROP TABLE ENCUESTA.tblEncuestaBloqueAsignatura

DROP TABLE ENCUESTA.tblEncuestaBloquePlantilla

DROP TABLE ENCUESTA.tblTipoEncuestado

DROP TABLE ENCUESTA.tblTipoEncuesta

DROP TABLE ENCUESTA.tblEncuestaPlantilla

DROP TABLE ENCUESTA.tblEncuestaAsignaturaAlumno

DROP TABLE ENCUESTA.tblEncuestaAsignatura

DROP SCHEMA ENCUESTA
