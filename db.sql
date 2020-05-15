-- Created by Vertabelo (http://vertabelo.com)
-- Last modification date: 2020-03-22 07:12:20.376

--PROCEDURE
--Create view
go
create view findMinEnrollmentN as
select coalesce(min(t1.IdEnrollment) + 1, 0)as id from EnrollmentN t1
left outer join EnrollmentN t2 on t1.IdEnrollment = t2.IdEnrollment - 1
where t2.IdEnrollment is null;
go

go
create view findMinStudentn as
select coalesce(min(t1.IdStudent) + 1, 0)as id from StudentN t1
left outer join StudentN t2 on t1.IdStudent = t2.IdStudentN - 1
where t2.IdStudent is null;
go


go
create procedure promoteStudent
@studies varchar(250), @semester int 
as
begin
declare @counter int, @idNew int, @idOld int, @id int, @idS int, @idE int
select @idOld = EnrollmentN.IdEnrollment from EnrollmentN 
where EnrollmentN.Semester = @semester and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies)
select @counter = COUNT(*) from EnrollmentN 
where EnrollmentN.Semester = @semester+1 and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies)
if (@counter = 1) begin
select @idNew = EnrollmentN.IdEnrollment from EnrollmentN 
where EnrollmentN.Semester = @semester+1 and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies)
update StudentN
set IdEnrollment = @idNew
where StudentN.IdEnrollment = @idOld;
end;
if (@counter > 1) begin
print 'ERROR find more than 1 semester on this studies'
end;
if(@counter = 0) begin
select @id = findMinEnrollmentN.id from findMinEnrollmentN;
select @idS = StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies;
insert into EnrollmentN values (@id, @semester+1,@idS, GETDATE());
update StudentN
set IdEnrollment= @id
where StudentN.IdEnrollment = @idOld;
end;
end;
go

--Create Tables

-- tables
-- Table: Enrollment
CREATE TABLE EnrollmentN (
    IdEnrollment int  NOT NULL,
    Semester int  NOT NULL,
    IdStudy int  NOT NULL,
    StartDate date  NOT NULL,
    CONSTRAINT Enrollment_pk PRIMARY KEY  (IdEnrollment)
);

-- Table: Student
CREATE TABLE StudentN (
    IndexNumber nvarchar(100)  NOT NULL,
    FirstName nvarchar(100)  NOT NULL,
    LastName nvarchar(100)  NOT NULL,
    BirthDate date  NOT NULL,
    IdEnrollment int  NOT NULL,
    CONSTRAINT Student_pkN PRIMARY KEY  (IndexNumber)
);

-- Table: Studies
CREATE TABLE StudiesN (
    IdStudy int  NOT NULL,
    Name nvarchar(100)  NOT NULL,
    CONSTRAINT Studies_pkN PRIMARY KEY  (IdStudy)
);

-- foreign keys
-- Reference: Enrollment_Studies (table: Enrollment)
ALTER TABLE EnrollmentN ADD CONSTRAINT Enrollment_StudiesN
    FOREIGN KEY (IdStudy)
    REFERENCES StudiesN (IdStudy);

-- Reference: Student_Enrollment (table: Student)
ALTER TABLE StudentN ADD CONSTRAINT Student_Enrollment
    FOREIGN KEY (IdEnrollment)
    REFERENCES EnrollmentN (IdEnrollment);

-- End of file.-- For work
insert into StudiesN values (1,'IT');
insert into StudiesN values (3,'ITE');
insert into EnrollmentN values (1,1,1,'12/12/2015');
insert into StudentN values ('s18693','Halina','Jajeworowa','12/12/2015',1);

select * from StudentN;
select * from EnrollmentN;
select * from StudiesN;

delete from EnrollmentN where EnrollmentN.IdEnrollment = 3;

select StudentN.FirstName, StudentN.LastName, EnrollmentN.Semester from StudentN
inner join EnrollmentN on StudentN.IdEnrollment = EnrollmentN.IdEnrollment
where StudentN.IndexNumber = 's18693';

select StudiesN.IdStudy from StudiesN where StudiesN.Name = 'IT';

select COUNT(*) from EnrollmentN where EnrollmentN.Semester = 1 and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like 'IT')

delete from EnrollmentN where EnrollmentN.IdEnrollment = 3;

drop procedure promoteStudent

go
create procedure promoteStudent
@studies varchar(250), @semester int 
as
begin
declare @counter int, @idNew int, @idOld int, @id int, @idS int, @idE int
select @idOld = EnrollmentN.IdEnrollment from EnrollmentN where EnrollmentN.Semester = @semester and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies)
select @counter = COUNT(*) from EnrollmentN where EnrollmentN.Semester = @semester+1 and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies)
if (@counter = 1) begin
select @idNew = EnrollmentN.IdEnrollment from EnrollmentN where EnrollmentN.Semester = @semester+1 and EnrollmentN.IdStudy = (select StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies)
update StudentN
set IdEnrollment = @idNew
where StudentN.IdEnrollment = @idOld;
end;
if (@counter > 1) begin
print 'ERROR find more than 1 semester on this studies'
end;
if(@counter = 0) begin
select @id = findMinEnrollmentN.id from findMinEnrollmentN;
select @idS = StudiesN.IdStudy from StudiesN where StudiesN.Name like @studies;
insert into EnrollmentN values (@id, @semester+1,@idS, GETDATE());
update StudentN
set IdEnrollment= @id
where StudentN.IdEnrollment = @idOld;
end;
end;
go

exec promoteStudent 'IT', 1

update StudentN
set IdEnrollment = 1;

delete from StudentN
where FirstName like 'Andrzej';

select EnrollmentN.IdEnrollment from EnrollmentN where EnrollmentN.Semester = 1 and EnrollmentN.IdStudy = 1;
--Nie dziala, id is null
insert into EnrollmentN (Semester,IdStudy,StartDate) values (1,1,'12/12/2015');

insert into EnrollmentN values (3,1,1,'12/12/2015');
delete from EnrollmentN where EnrollmentN.IdEnrollment = 2;

drop view findMinEnrollmentN;

go
create view findMinEnrollmentN as
select coalesce(min(t1.IdEnrollment) + 1, 0)as id from EnrollmentN t1
left outer join EnrollmentN t2 on t1.IdEnrollment = t2.IdEnrollment - 1
where t2.IdEnrollment is null;
go

select * from findMinEnrollmentN;

exec findMinEnrollmentN;


select StudentN.IndexNumber from StudentN where StudentN.IndexNumber like 's1234';

select * from EnrollmentN where EnrollmentN.IdEnrollment = 1;

select EnrollmentN.IdEnrollment from EnrollmentN 
inner join StudiesN on EnrollmentN.IdStudy = StudiesN.IdStudy and StudiesN.Name like 'IT'
where EnrollmentN.Semester = 1;

--------------cw 6
select * from StudentN where StudentN.IndexNumber = 1;

select * from StudentN;

---cw 7 ---
select * from StudentN;

alter table StudentN
add Password varchar(255);

alter table StudentN
drop column Password;

update StudentN
set Password = 'a'

select StudentN.Password from StudentN where StudentN.IndexNumber like 's18693';