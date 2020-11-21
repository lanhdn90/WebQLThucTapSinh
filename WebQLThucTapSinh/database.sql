CREATE TABLE Role(
	RoleID INT IDENTITY (1,1) NOT NULL,
	NameRole NVARCHAR(250) NULL,
	PRIMARY KEY (RoleID)
);

CREATE TABLE Menu(
	Id INT IDENTITY (1,1) NOT NULL,
	TextMenu NVARCHAR(250) NULL,
	Link VARCHAR(250) NULL,
	RoleID INT NULL,
	PRIMARY KEY(Id),
	CONSTRAINT FK_Menu_Role FOREIGN KEY (RoleID) REFERENCES dbo.Role(RoleID)
);

--Thêm Ngày gia hạn, hạn sử dụng, status--
CREATE TABLE Organization(
	ID VARCHAR(8) NOT NULL,
	Name NVARCHAR(500) NULL,
	Address NVARCHAR(500) NULL,
	Phone VARCHAR(11) NULL,
	Fax VARCHAR(11) NULL,
	Image VARCHAR(250) NULL,
	Logo VARCHAR(250) NULL,
	Note NVARCHAR(500) NULL,
	Email VARCHAR(250) NULL,
	StartDay DATETIME NULL,
	ExpiryDate INT DEFAULT (1) NULL,
	Status BIT DEFAULT (0) NULL,
	SendEmail BIT DEFAULT (0) NULL,
	PRIMARY KEY(ID)
);

CREATE TABLE Person(
	PersonID VARCHAR(8)  NOT NULL,
	LastName NVARCHAR(50) NULL,
	FirstName NVARCHAR(50) NULL,
	Birthday DATE NULL,
	Gender BIT NULL,
	Address NVARCHAR(500) NULL,
	Phone VARCHAR(10) NULL,
	Email VARCHAR(250) NULL,
	Image VARCHAR(250) NULL,
	RoleID INT NULL,
	CompanyID VARCHAR(8) NULL,
	SchoolID VARCHAR(8) NULL,
	PRIMARY KEY(PersonID),
	CONSTRAINT FK_Person_Role FOREIGN KEY (RoleID) REFERENCES dbo.Role(RoleID),
	CONSTRAINT FK_Person_Company FOREIGN KEY (CompanyID) REFERENCES dbo.Organization(ID),
	CONSTRAINT FK_Person_School FOREIGN KEY (SchoolID) REFERENCES dbo.Organization(ID)
);

CREATE TABLE Users(
	UserName Varchar(50) NOT NULL,
	PersonID VARCHAR(8) UNIQUE FOREIGN KEY REFERENCES dbo.Person(PersonID),
	PassWord VARCHAR(50) NULL,
	Status BIT DEFAULT (0) NULL,
	PRIMARY KEY(UserName),
	
);

CREATE TABLE InternShip(
	InternshipID INT NOT NULL,
	CourseName NVARCHAR(250) NULL,
	Note NVARCHAR(500) NULL,
	PersonID VARCHAR(8) NULL,
	CompanyID VARCHAR(8) NULL,
	StartDay DATETIME NULL,
	ExpiryDate INT DEFAULT (1) NULL,
	Status BIT DEFAULT (0) NULL,
	PRIMARY KEY (InternshipID),
	CONSTRAINT FK_InternShip_Person FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID),
	CONSTRAINT FK_InternShip_Company FOREIGN KEY (CompanyID) REFERENCES dbo.Organization(ID)

);

CREATE TABLE Task(
	TaskID INT NOT NULL,
	TaskName NVARCHAR(250) NULL,
	Note NVARCHAR(500) NULL,
	Video VARCHAR(250) NULL,
	PersonID VARCHAR(8) NULL,
	NumberOfQuestions INT NULL,
	Result INT NULL,
	PRIMARY KEY (TaskID),
	CONSTRAINT FK_Task_Person FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID)
);

CREATE TABLE IntershipWithTask(
	ID INT NOT NULL,
	InternshipID INT NULL,
	TaskID INT NULL,
	Sort INT NULL,
	PRIMARY KEY (ID),
	CONSTRAINT FK_IntershipWithTask_Task FOREIGN KEY (TaskID) REFERENCES dbo.Task(TaskID),
	CONSTRAINT FK_IntershipWithTask_Internship FOREIGN KEY (InternshipID) REFERENCES dbo.InternShip(InternshipID)
);

CREATE TABLE Question(
	QuestionID INT NOT NULL,
	TaskID INT NULL,
	Content NVARCHAR(500) NULL,
	Answer VARCHAR(10) NULL,
	A NVARCHAR(300) NULL,
	B NVARCHAR(300) NULL,
	C NVARCHAR(300) NULL,
	D NVARCHAR(300) NULL,
	PRIMARY KEY(QuestionID),
	CONSTRAINT FK_Question_Task FOREIGN KEY (TaskID) REFERENCES dbo.Task(TaskID)
);

--Thêm mã sinh viên--
CREATE TABLE Intern(
	PersonID VARCHAR(8) UNIQUE FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID),
	StudentCode VARCHAR(15) NULL,
	InternshipID INT NULL,
	Result INT NULL,
	PRIMARY KEY (PersonID),
	CONSTRAINT FK_Intern_InternShip FOREIGN KEY (InternshipID) REFERENCES dbo.InternShip(InternshipID),
);

CREATE TABLE TestResults(
	ID INT NOT NULL,
	PersonID VARCHAR(8) NULL,
	TaskID INT NULL,
	Answer INT NULL,
	PRIMARY KEY(ID),
	CONSTRAINT FK_TestResults_Task FOREIGN KEY (TaskID) REFERENCES dbo.Task(TaskID),
	CONSTRAINT FK_TestResults_Intern FOREIGN KEY (PersonID) REFERENCES dbo.Intern(PersonID)
	
);

CREATE TABLE News (
    ID      INT            NOT NULL,
    PersonID VARCHAR(8) NULL,
    Header     NVARCHAR (250) NULL,
    Note NVARCHAR(500) NULL,
    Image VARCHAR(250) NULL,
    Postdate   DATETIME       NULL,
    PRIMARY KEY (ID),
	CONSTRAINT FK_News_Person FOREIGN KEY (PersonID) REFERENCES dbo.Person(PersonID)
);

INSERT INTO dbo.Role(NameRole) VALUES  ('Admin' ), ('Manager' ), ('Faculty'), ('Leader'),('Interns'), ('School');

--Thêm tài khoản Admin--
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday, Phone ,Email ,RoleID )
VALUES  ( 'ZXCVBNML' , N'Admin' ,N'Admin' ,GETDATE() ,'0905865877' ,'adminstrator@gmail.com' ,1);	

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Admin','e10adc3949ba59abbe56e057f20f883e','ZXCVBNML',1);

--Công ty Axon --

--Thêm Company --
INSERT INTO dbo.Organization (ID,Name,Address,Phone, Email, StartDay)
VALUES('QWERTFGH', N'Công ty AXon', N'Đường 2 tháng 9',0935123456, 'Axondn@Gmail.com', GETDATE());

--Thêm tài khoản Manager--  CompanyID = "QWERTFGH"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCVBHML' , N'Đỗ Trường' , N'Thuận' ,GETDATE() ,'0905194913' ,'thuando@gmail.com' ,2 ,'QWERTFGH' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Axondn','e10adc3949ba59abbe56e057f20f883e','ZXCVBHML',1);

INSERT INTO dbo.InternShip( InternshipID ,CourseName ,CompanyID ,StartDay ,ExpiryDate ,Status)
VALUES  ( 1 ,N'English Language' ,'QWERTFGH' ,GETDATE() ,3 ,0 )

--Thêm tài khoản Ledder -- CompanyID = "QWERTFGH"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCKBHML' , N'Tạ Ngọc Anh' , N'Khôi' ,GETDATE() ,'0702715537' ,'AnhKhoidn@gmail.com' ,4 ,'QWERTFGH' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('anhkhoidn','e10adc3949ba59abbe56e057f20f883e','ZXCKBHML',1);

INSERT INTO dbo.InternShip( InternshipID ,CourseName , PersonID ,CompanyID ,StartDay ,ExpiryDate ,Status )
VALUES  ( 2 ,N'Back-End' ,'ZXCKBHML' ,'QWERTFGH' ,GETDATE() ,3 ,0 )

INSERT INTO dbo.Task( TaskID ,TaskName ,Video ,PersonID )
VALUES  ( 1 ,
          N'Ngôn Ngữ Lập Trình' ,
          '<iframe width="949" height="534" src="https://www.youtube.com/embed/Hmp6v6crcf8" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>' , -- Video - varchar(250)
          'ZXCKBHML' )

INSERT INTO dbo.Task( TaskID ,TaskName ,Video ,PersonID )
VALUES  ( 2 ,
          N'Back-end từ cơ bản đến nâng cao' ,
          '<iframe width="949" height="534" src="https://www.youtube.com/embed/33jW9PLId90" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>' ,
          'ZXCKBHML' )

INSERT INTO dbo.IntershipWithTask ( ID, InternshipID, TaskID, SORT )VALUES  ( 1,2,1, 1 )
INSERT INTO dbo.IntershipWithTask ( ID, InternshipID, TaskID, SORT )VALUES  ( 2,2,2, 2 )

--Thêm tài khoản Ledder -- CompanyID = "QWERTFGH"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCCBHML' , N'Tạ  Nguyễn Ngọc' , N'Hiếu' ,GETDATE() ,'0905643120' ,'Hieutadn@gmail.com' ,4 ,'QWERTFGH' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('tahieudn','e10adc3949ba59abbe56e057f20f883e','ZXCCBHML',1);

INSERT INTO dbo.InternShip( InternshipID ,CourseName , PersonID ,CompanyID ,StartDay ,ExpiryDate ,Status )
VALUES  ( 3 ,N'Front-End' ,'ZXCCBHML' ,'QWERTFGH' ,GETDATE() ,3 ,1 )

INSERT INTO dbo.Task( TaskID ,TaskName ,Video ,PersonID )
VALUES  ( 3 ,
          N'Tổng quan HTML' ,
          '<iframe width="949" height="507" src="https://www.youtube.com/embed/cA_My615iFk" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>' , -- Video - varchar(250)
          'ZXCCBHML' )

INSERT INTO dbo.Task( TaskID ,TaskName ,Video ,PersonID )
VALUES  ( 4 ,
          N'Tổng quan CSS' ,
          '<iframe width="949" height="507" src="https://www.youtube.com/embed/z699narBw_A" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>' ,
          'ZXCCBHML' )

INSERT INTO dbo.IntershipWithTask ( ID, InternshipID, TaskID, SORT )VALUES  ( 3,3,3, 1 )
INSERT INTO dbo.IntershipWithTask ( ID, InternshipID, TaskID, SORT )VALUES  ( 4,3,3, 2 )

-- Thêm câu hỏi --
INSERT INTO dbo.Question(QuestionID,TaskID,Content,Answer,A,B,C,D)VALUES(1, 3,N'Ngôn ngữ c sharp kí hiệu là gì?','A',N'A.C#',N'B.C++', N'C.C', N'D.CC');



---Thêm Thực tập sinh-- CompanyID = "QWERTFGH"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCDBUNQ' , N'Huỳnh Thị Ngọc' , N'Chinh' ,GETDATE() ,'0703315537' ,'chinhhuynhdn@gmail.com' ,5 ,'QWERTFGH' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('chinhhuynhdn','e10adc3949ba59abbe56e057f20f883e','ZXCDBUNQ',1);

INSERT INTO dbo.Intern( PersonID ,StudentCode ,Result )VALUES  ( 'ZXCDBUNQ' ,'2221128202' ,0 )


---Thêm Thực tập sinh-- CompanyID = "QWERTFGH"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCDRRNQ' , N'Huỳnh Thị Mỹ' , N'Linh' ,GETDATE() ,'0703315537' ,'linhhuynhdn@gmail.com' ,5 ,'QWERTFGH' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('linhhuynhdn','e10adc3949ba59abbe56e057f20f883e','ZXCDRRNQ',1);

INSERT INTO dbo.Intern( PersonID ,StudentCode ,InternshipID ,Result )VALUES  ( 'ZXCDRRNQ' ,'2221133202' ,3 ,0 )




--Công ty Lumina --

--Thêm Company --
INSERT INTO dbo.Organization (ID,Name,Address,Phone, Email, StartDay)
VALUES('QWERGDEM', N'Công ty Lumina', N'Đường 30 tháng 4',0935234567, 'Luminadn@Gmail.com', GETDATE());

--Thêm tài khoản Manager--  CompanyID = "QWERGDEM"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCVBDML' , N'Ngô Văn' , N'Duy' ,GETDATE() ,'0905194930' ,'duyngodn@gmail.com' ,2 ,'QWERGDEM' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('duyngodn','e10adc3949ba59abbe56e057f20f883e','ZXCVBDML',1);

--Thêm tài khoản Ledder -- CompanyID = "QWERGDEM"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCQBHML' , N'Phạm Phú Minh' , N'Hiếu' ,GETDATE() ,'0702715577' ,'hieuphamdn@gmail.com' ,4 ,'QWERGDEM' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('hieuphamdn','e10adc3949ba59abbe56e057f20f883e','ZXCQBHML',1);

--Thêm tài khoản Ledder -- CompanyID = "QWERGDEM"
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCABHML' , N'Võ Ngọc' , N'Lân' ,GETDATE() ,'0905643200' ,'lanvodn@gmail.com' ,4 ,'QWERGDEM' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('lanvodn','e10adc3949ba59abbe56e057f20f883e','ZXCABHML',1);


--Duy Tân--
-- Thêm School --
INSERT INTO dbo.Organization (ID,Name,Address,Phone, Email, StartDay,ExpiryDate)
VALUES('ASDFGHJR', N'Đại học Duy Tân', N'Đường Nguyễn Văn Linh',0935345678, 'Duytandn@Gmail.com', GETDATE(),1);

--Thêm tài khoản School-- SchoolId = 'ASDFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'ZXCVBUML' , N'Nguyễn Minh' , N'Thắng' ,GETDATE() ,'0905195530' ,'thangnguyendn@gmail.com' ,6 ,'ASDFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('thangnguyendn','e10adc3949ba59abbe56e057f20f883e','ZXCVBUML',1);

-- Thêm Khoa --
INSERT INTO dbo.Organization (ID,Name,Address,Phone, Email, StartDay,ExpiryDate)
VALUES('BBDFGHJR', N'Khoa CNTT', N'Đường Quang Trung',0935345678, 'CnttDuytandn@Gmail.com', GETDATE(),1);

--Thêm tài khoản Faculty--  CompanyId ='ASDFGHJR'  SchoolId = 'BBDFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID,SchoolID)
VALUES  ( 'ZXCNNUML' , N'Nguyễn Đình' , N'Kiên' ,GETDATE() ,'0905145530' ,'kiennguyendn@gmail.com' ,3 ,'ASDFGHJR','BBDFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('kiennguyendn','e10adc3949ba59abbe56e057f20f883e','ZXCNNUML',1);

--Thêm tài khoản Faculty--  CompanyId ='ASDFGHJR'  SchoolId = 'BBDFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID,SchoolID)
VALUES  ( 'ZXDUNUML' , N'Trịnh Tấn' , N'Tài' ,GETDATE() ,'0905145588' ,'tantaidn@gmail.com' ,3 ,'ASDFGHJR','BBDFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('tantaidn','e10adc3949ba59abbe56e057f20f883e','ZXDUNUML',1);

---Thêm Thực tập sinh-- CompanyID = "QWERTFGH" SchoolId = 'BBDFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID,SchoolID)
VALUES  ( 'ZXCOOUNQ' , N'Doãn Công' , N'Thức' ,GETDATE() ,'0905315537' ,'thucdoandn@gmail.com' ,5 ,'QWERTFGH','BBDFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('thucdoandn','e10adc3949ba59abbe56e057f20f883e','ZXCOOUNQ',1);

INSERT INTO dbo.Intern( PersonID ,StudentCode ,Result )VALUES  ( 'ZXCOOUNQ' ,'2221133202' ,0 )


---Thêm Thực tập sinh-- CompanyID = "QWERTFGH" SchoolId = 'BBDFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID,SchoolID)
VALUES  ( 'ZXCOFSNQ' , N'Lê Hoàng' , N'Minh' ,GETDATE() ,'0935315537' ,'Minhhoangdn@gmail.com' ,5 ,'QWERTFGH','BBDFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('Minhhoangdn','e10adc3949ba59abbe56e057f20f883e','ZXCOFSNQ',1);

INSERT INTO dbo.Intern( PersonID ,StudentCode ,InternshipID ,Result )VALUES  ( 'ZXCOFSNQ' ,'2222833202' ,3 ,0 )


--Đại học FPT--
-- Thêm School --
INSERT INTO dbo.Organization (ID,Name,Address,Phone, Email, StartDay,ExpiryDate)
VALUES('ASDFHHJR', N'Đại học FPT', N'Đường Nam Kỳ Khởi Nghĩa',0935456789, 'Fptcompanydn@Gmail.com', GETDATE(),1);

--Thêm tài khoản School-- SchoolId = 'ASDFHHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID)
VALUES  ( 'NHCVBUML' , N'Nguyễn Kim' , N'Tâm' ,GETDATE() ,'0905905530' ,'kiemtamdn@gmail.com' ,6 ,'ASDFHHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('kiemtamdn','e10adc3949ba59abbe56e057f20f883e','NHCVBUML',1);

-- Thêm Khoa --
INSERT INTO dbo.Organization (ID,Name,Address,Phone, Email, StartDay,ExpiryDate)
VALUES('BUIFGHJR', N'Khoa CNTT', N'Đường Nam Kỳ Khởi Nghĩa',0935995678, 'CnttFPTdn@Gmail.com', GETDATE(),1);

--Thêm tài khoản Faculty--  CompanyId ='ASDFHHJR'  SchoolId = 'BUIFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID,SchoolID)
VALUES  ( 'ZXCNPPML' , N'Nguyễn Văn',N'Trung' ,GETDATE() ,'0905187530' ,'trungnguyendn@gmail.com' ,3 ,'ASDFHHJR','BUIFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('trungnguyendn','e10adc3949ba59abbe56e057f20f883e','ZXCNPPML',1);

--Thêm tài khoản Faculty--  CompanyId ='ASDFHHJR'  SchoolId = 'BUIFGHJR'
INSERT INTO dbo.Person( PersonID ,LastName ,FirstName ,Birthday , Phone ,Email ,RoleID ,CompanyID,SchoolID)
VALUES  ( 'ZXDUOUML' , N'Phan Văn' , N'Trung' ,GETDATE() ,'0905325588' ,'trungphandn@gmail.com' ,3 ,'ASDFHHJR','BUIFGHJR' )

INSERT INTO dbo.Users (UserName,PassWord,PersonID,Status) VALUES ('trungphandn','e10adc3949ba59abbe56e057f20f883e','ZXDUOUML',1);