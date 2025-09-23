

CREATE OR REPLACE PROCEDURE SSP_LISTAR_TIPO_PROGRAMA (
    p_cursor OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN p_cursor FOR
        SELECT
            DISTINCT SSBSECT_SUBJ_CODE AS TIPO_PROGRAMA_ID,
                   (CASE WHEN SSBSECT_SUBJ_CODE = 'PSDP' THEN 'DIPLOMAS'
                         WHEN SSBSECT_SUBJ_CODE = 'PSEV' THEN 'EVENTOS'
                         WHEN SSBSECT_SUBJ_CODE = 'PSMA' THEN 'MAESTRIA'
                         WHEN SSBSECT_SUBJ_CODE = 'PSPA' THEN 'PASANTIA'
                         WHEN SSBSECT_SUBJ_CODE = 'PSPE' THEN 'PROGRAMAS DE ESPECIALIZACION'
                         WHEN SSBSECT_SUBJ_CODE = 'PSCC' THEN 'CURSO CERRADO'
                         WHEN SSBSECT_SUBJ_CODE = 'PSCU' THEN 'CURSO'
                         WHEN SSBSECT_SUBJ_CODE = 'PSDI' THEN 'DIPLOMADO'
                         WHEN SSBSECT_SUBJ_CODE = 'PSDO' THEN 'DOCTORADO' END) AS NOMBRE_PROGRAMA
               FROM SSBSECT
               WHERE SSBSECT_SUBJ_CODE IN (  'PSDP'
                                            ,'PSEV'
                                            ,'PSMA'
                                            ,'PSPA'
                                            ,'PSPE'
                                            ,'PSCC'
                                            ,'PSCU'
                                            ,'PSDI'
                                            ,'PSDO');
END;
/


CREATE OR REPLACE PROCEDURE SSP_LISTAR_SECCIONES (
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

CREATE OR REPLACE PROCEDURE SSP_LISTAR_PERIODOS (
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


CREATE OR REPLACE PROCEDURE SSP_LISTAR_SEDES (
    P_CURSOR OUT SYS_REFCURSOR
)
AS
BEGIN
    OPEN P_CURSOR FOR
        SELECT STVCAMP_CODE AS SEDE_ID,
               STVCAMP_DESC AS NOMBRE_SEDE
        FROM STVCAMP
        ORDER BY STVCAMP_CODE;
END SSP_LISTAR_SEDES;
/

CREATE OR REPLACE PROCEDURE SSP_LISTAR_ASIGNATURAS (
    p_blck_code IN VARCHAR2,
    p_cursor    OUT SYS_REFCURSOR
) IS
BEGIN
    OPEN p_cursor FOR
        SELECT DISTINCT
               T2.SFRSTCR_CRN AS NRC,
               T4.scrsyln_long_course_title AS NOMBRE_ASIGNATURA
        FROM SSRBLCK T1
        INNER JOIN SFRSTCR T2 
            ON T1.SSRBLCK_TERM_CODE = T2.SFRSTCR_TERM_CODE
           AND T1.SSRBLCK_CRN = T2.SFRSTCR_CRN
           AND T1.SSRBLCK_BLCK_CODE = p_blck_code
        INNER JOIN SSBSECT T3 
            ON T3.SSBSECT_TERM_CODE = T2.SFRSTCR_TERM_CODE
           AND T3.SSBSECT_CRN = T2.SFRSTCR_CRN
        INNER JOIN SCRSYLN T4 
            ON T4.scrsyln_subj_code = T3.SSBSECT_SUBJ_CODE
           AND T4.scrsyln_crse_numb = T3.SSBSECT_CRSE_NUMB;
END;
/




