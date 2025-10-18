
/*
--  FILE NAME..: encuestas_oracle.sqlencuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_TIPO_PROGRAMA
--  PRODUCT....: (Listar los tipos de programas para creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_TIPO_PROGRAMA (
    P_BLCK_CODE IN VARCHAR2,
    O_CURSOR    OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN O_CURSOR FOR
        SELECT DISTINCT
               COALESCE(T5.SMRPRLE_PROGRAM, 'SIN ID') AS TIPO_PROGRAMA_ID,
               COALESCE(T5.SMRPRLE_DESCRIPTION, 'SIN NOMBRE') AS NOMBRE_PROGRAMA
        FROM SSRBLCK T1
        INNER JOIN SFRSTCR T2 ON T1.SSRBLCK_TERM_CODE = T2.SFRSTCR_TERM_CODE
           AND T1.SSRBLCK_CRN = T2.SFRSTCR_CRN
           AND T1.SSRBLCK_BLCK_CODE = P_BLCK_CODE
        INNER JOIN SFRENSP T3 ON T2.SFRSTCR_TERM_CODE = T3.SFRENSP_TERM_CODE
           AND T2.SFRSTCR_PIDM = T3.SFRENSP_PIDM
        INNER JOIN SORLCUR T4 ON T4.SORLCUR_PIDM = T3.SFRENSP_PIDM
        INNER JOIN SMRPRLE T5 ON T5.SMRPRLE_PROGRAM = T4.SORLCUR_PROGRAM;
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_SECCIONES
--  PRODUCT....: (Listar las secciones para creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_SECCIONES (
    p_cursor OUT SYS_REFCURSOR
) IS
    v_max_anio   VARCHAR2(2);
    v_prev_anio  VARCHAR2(2);
BEGIN

    SELECT MAX(SUBSTR(SSRBLCK_BLCK_CODE, 1, 2))
    INTO v_max_anio
    FROM SATURN.SSRBLCK
    WHERE REGEXP_LIKE(SUBSTR(SSRBLCK_BLCK_CODE,1,2), '^[0-9]+$');

    v_prev_anio := TO_CHAR(TO_NUMBER(v_max_anio) - 1);

    OPEN p_cursor FOR
        SELECT DISTINCT 
               --SSRBLCK_TERM_CODE,
                COALESCE(BLCK.SSRBLCK_BLCK_CODE, 'SIN_ID') AS SECCION_ID,
                COALESCE(PR.SMRPRLE_DESCRIPTION, 'SIN_DESCRIPCION') AS NOMBRE_SECCION
        FROM SATURN.SSRBLCK BLCK
        INNER JOIN SATURN.SMRPRLE PR ON PR.SMRPRLE_PROGRAM = SUBSTR(BLCK.SSRBLCK_BLCK_CODE, 3, 3)
        WHERE 1 = 1
            --PR.SMRPRLE_LEVL_CODE IN ('PS','EC','CO')
            AND (SUBSTR(SSRBLCK_BLCK_CODE, 6, 2) = 'TV'
                OR SSRBLCK_BLCK_CODE LIKE '%CP%')
          AND SUBSTR(SSRBLCK_BLCK_CODE, 1, 2) IN (v_max_anio, v_prev_anio);
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_PERIODOS
--  PRODUCT....: (Listar periodos para creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_PERIODOS (
    p_cursor OUT SYS_REFCURSOR
) IS
    v_max_year NUMBER;
    v_min_year NUMBER;
    v_cutoff   NUMBER := TO_NUMBER(TO_CHAR(SYSDATE,'YYYY'));
BEGIN
    SELECT MAX(TO_NUMBER(SUBSTR(STVTERM_CODE,1,4)))
    INTO v_max_year
    FROM STVTERM
    WHERE REGEXP_LIKE(SUBSTR(STVTERM_CODE,1,4), '^[0-9]{4}$')
      AND TO_NUMBER(SUBSTR(STVTERM_CODE,1,4)) BETWEEN 1900 AND v_cutoff;

    IF v_max_year IS NULL THEN
        OPEN p_cursor FOR SELECT STVTERM_CODE, STVTERM_DESC FROM STVTERM WHERE 1 = 0;
        RETURN;
    END IF;

    v_min_year := v_max_year - 1;

    OPEN p_cursor FOR
      SELECT
            COALESCE(STVTERM_CODE, 'SIN_CODIGO') AS PERIODO_ID,
            COALESCE(STVTERM_DESC, 'SIN_DESCRIPCION')AS NOMBRE_PERIODO
      FROM STVTERM
      WHERE REGEXP_LIKE(SUBSTR(STVTERM_CODE,1,4), '^[0-9]{4}$')
        AND TO_NUMBER(SUBSTR(STVTERM_CODE,1,4)) IN (v_max_year, v_min_year)
      ORDER BY STVTERM_CODE DESC;
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_SEDES
--  PRODUCT....: (Listar las sedes para la creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_SEDES (
    P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR
        SELECT STVCAMP_CODE AS SEDE_ID,
               STVCAMP_DESC AS NOMBRE_SEDE
        FROM STVCAMP
        ORDER BY STVCAMP_CODE;
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_ASIGNATURAS
--  PRODUCT....: (Listar las asignaturas para la creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_ASIGNATURAS (
    p_blck_code   IN VARCHAR2,
    p_program_id  IN VARCHAR2,
    p_cursor      OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN p_cursor FOR
        SELECT DISTINCT
               T2.SFRSTCR_CRN AS NRC,
               T4.scrsyln_long_course_title AS NOMBRE_ASIGNATURA
        FROM SSRBLCK T1
        INNER JOIN SFRSTCR T2 ON T1.SSRBLCK_TERM_CODE = T2.SFRSTCR_TERM_CODE
           AND T1.SSRBLCK_CRN = T2.SFRSTCR_CRN
           AND T1.SSRBLCK_BLCK_CODE = p_blck_code
        INNER JOIN SSBSECT T3 ON T3.SSBSECT_TERM_CODE = T2.SFRSTCR_TERM_CODE
           AND T3.SSBSECT_CRN = T2.SFRSTCR_CRN
        INNER JOIN SCRSYLN T4 ON T4.scrsyln_subj_code = T3.SSBSECT_SUBJ_CODE
           AND T4.scrsyln_crse_numb = T3.SSBSECT_CRSE_NUMB
        INNER JOIN SFRENSP T5 ON T2.SFRSTCR_TERM_CODE = T5.SFRENSP_TERM_CODE
           AND T2.SFRSTCR_PIDM = T5.SFRENSP_PIDM
        INNER JOIN SORLCUR T6 ON T6.SORLCUR_PIDM = T5.SFRENSP_PIDM
        INNER JOIN SMRPRLE T7 ON T7.SMRPRLE_PROGRAM = T6.SORLCUR_PROGRAM
        WHERE p_program_id IS NULL 
              OR p_program_id = '' 
              OR T7.SMRPRLE_PROGRAM = p_program_id;
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_DOCENTES_ASIGNATURA
--  PRODUCT....: (Listar los docentes por asignatura para la creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_DOCENTES_ASIGNATURA (
    p_seccion    IN VARCHAR2,
    p_asignatura IN VARCHAR2,
    p_cursor     OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN p_cursor FOR
		SELECT DISTINCT
               T3.spriden_id AS PIDM
               ,T3.spriden_last_name || ' ' || T3.spriden_first_name AS DOCENTE
               --,T4.ssrblck_blck_code AS SECCION
               --,T5.SFRSTCR_CRN  AS ASIGNATURA
               --,COALESCE(T7.SMRPRLE_PROGRAM, 'SIN ID')       AS TIPO_PROGRAMA_ID
               --,COALESCE(T7.SMRPRLE_DESCRIPTION, 'SIN NOMBRE') AS NOMBRE_PROGRAMA
        FROM ssbsect T1
        INNER JOIN sirasgn T2 ON T2.sirasgn_term_code = T1.ssbsect_term_code
           AND T2.sirasgn_crn = T1.ssbsect_crn
        INNER JOIN spriden T3 ON T3.spriden_pidm = T2.sirasgn_pidm
           AND T3.spriden_change_ind IS NULL
        INNER JOIN ssrblck T4 ON T4.ssrblck_term_code = T1.ssbsect_term_code
           AND T4.ssrblck_crn = T1.ssbsect_crn
        -- Enlace con inscripción de alumnos
        INNER JOIN sfrstcr T5 ON T5.sfrstcr_term_code = T1.ssbsect_term_code
           AND T5.sfrstcr_crn       = T1.ssbsect_crn
        INNER JOIN sfrensp T6 ON T6.sfrensp_term_code = T5.sfrstcr_term_code
           AND T6.sfrensp_pidm      = T5.sfrstcr_pidm
        INNER JOIN sorlcur T8 ON T8.sorlcur_pidm = T6.sfrensp_pidm
        INNER JOIN smrprle T7 ON T7.smrprle_program = T8.sorlcur_program
        WHERE 1 = 1
        --AND T3.spriden_id = '08709520'
        --AND T4.ssrblck_blck_code = '21PMADM303'
        --AND T7.SMRPRLE_PROGRAM = 'MPB'
        --AND T5.SFRSTCR_CRN = '1860'
		AND (p_seccion IS NULL OR T4.ssrblck_blck_code = p_seccion)       -- filtro por sección
        AND (p_asignatura IS NULL OR T5.SFRSTCR_CRN = p_asignatura); -- filtro por asignatura
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_CORREOS_ENCUESTA
--  PRODUCT....: (Listar los correos de quienes van a recibir la encuesta creada)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_CORREOS_ENCUESTA (
    p_asignatura IN VARCHAR2,
    p_seccion    IN VARCHAR2,
    p_cursor     OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN p_cursor FOR
        SELECT DISTINCT
               T3.spriden_id AS ALUMNO_ID
			   ,T3.SPRIDEN_LAST_NAME ||' '|| T3.SPRIDEN_FIRST_NAME AS NOMBRE_ALUMNO
               --,T3.spriden_id || '@continental.edu.pe' AS CORREO_ALUMNO
        FROM ssbsect T1
        INNER JOIN sfrstcr T2 ON T2.sfrstcr_term_code = T1.ssbsect_term_code
           AND T2.sfrstcr_crn       = T1.ssbsect_crn
        INNER JOIN sfrensp T6 ON T6.sfrensp_term_code = T2.sfrstcr_term_code
           AND T6.sfrensp_pidm      = T2.sfrstcr_pidm
        INNER JOIN spriden T3 ON T3.spriden_pidm = T6.sfrensp_pidm
           AND T3.spriden_change_ind IS NULL
        INNER JOIN sorlcur T4 ON T4.sorlcur_pidm = T6.sfrensp_pidm
           AND T4.sorlcur_seqno = (
                  SELECT MAX(a.sorlcur_seqno)
                  FROM sorlcur a
                  WHERE a.sorlcur_pidm = T6.sfrensp_pidm
                    AND a.sorlcur_lmod_code = 'LEARNER'
                    AND a.sorlcur_cact_code = 'ACTIVE'
                    AND a.sorlcur_key_seqno = T6.sfrensp_key_seqno
                    AND a.sorlcur_term_code <= T6.sfrensp_term_code
               )
        INNER JOIN ssrblck T5 ON T5.ssrblck_term_code = T1.ssbsect_term_code
           AND T5.ssrblck_crn       = T1.ssbsect_crn
        INNER JOIN sgrstsp T7 ON T7.sgrstsp_pidm      = T6.sfrensp_pidm
           AND T7.sgrstsp_key_seqno = T4.sorlcur_key_seqno
        WHERE (p_asignatura IS NULL OR T2.sfrstcr_crn = p_asignatura)
          AND (p_seccion IS NULL OR T5.ssrblck_blck_code = p_seccion);
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_NOMBRE_DOCENTE_DNI
--  PRODUCT....: (Listar los nombres de los docentes para la creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_NOMBRE_DOCENTE_DNI (
    P_BLCK_CODE IN VARCHAR2,
    O_CURSOR    OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN O_CURSOR FOR
        SELECT DISTINCT 
               TRIM(
                   COALESCE(spriden_last_name, '') || ' ' ||
                   COALESCE(spriden_first_name, '')
               ) AS DOCENTE
        FROM spriden
        WHERE spriden_id = P_BLCK_CODE;
END;
/


/*
--  FILE NAME..: encuestas_oracle.sql
--  RELEASE....: 9.0 [U. CONTINENTAL 1.0]
--  OBJECT NAME: P_LISTAR_ASESORES
--  PRODUCT....: (Listar los nombres de los asesores para la creación de encuestas)
--  COPYRIGHT..: Copyright Copyright UNIVERSIDAD CONTINENTAL 2025
---------------------------------------------------------------------------
--  MOD     FECHA       USUARIO     MODIFICACION
---------------------------------------------------------------------------
    001     DD/MM/YYYY  
*/
CREATE OR REPLACE PROCEDURE P_LISTAR_ASESORES (
    P_BLCK_CODE IN VARCHAR2,
    O_CURSOR    OUT SYS_REFCURSOR
) AS
BEGIN
    OPEN O_CURSOR FOR
        SELECT DISTINCT
            --g.govsdav_pk_parenttab,
            g.govsdav_value_as_char AS ASESOR_ID
            ,s.spriden_first_name || ' ' || s.spriden_last_name AS NOMBRE_ASESOR
        FROM GOVSDAV g
        INNER JOIN SPRIDEN s ON s.SPRIDEN_ID = g.govsdav_value_as_char
        WHERE g.GOVSDAV_TABLE_NAME = 'STVBLCK'
        AND g.GOVSDAV_ATTR_NAME ='DNI_ASESOR'
        AND g.govsdav_pk_parenttab = P_BLCK_CODE;
END;
/

