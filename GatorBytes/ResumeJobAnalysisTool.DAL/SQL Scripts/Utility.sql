/*  Helpful scripts */


/* Drops db "currently in use" */
 use master 
 go
 alter database ResumeJobAnalysisTool set single_user with rollback immediate

 drop database ResumeJobAnalysisTool