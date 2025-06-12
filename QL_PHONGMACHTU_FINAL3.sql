CREATE DATABASE QL_PHONGMACHTU
GO

USE QL_PHONGMACHTU
GO
--------------------------CREATE TABLE--------------------------
CREATE TABLE CHUCNANG (
    ID_ChucNang INT PRIMARY KEY IDENTITY(1,1),
    TenChucNang NVARCHAR(100),
	TenManHinhDuocLoad NVARCHAR(100)
);
DROP TABLE CHUCNANG
CREATE TABLE NHOMNGUOIDUNG (
    ID_Nhom INT PRIMARY KEY IDENTITY(1,1),
    TenNhom NVARCHAR(100)
);

CREATE TABLE PHANQUYEN (
    ID_Nhom INT,
    ID_ChucNang INT,
    PRIMARY KEY (ID_Nhom, ID_ChucNang),
    FOREIGN KEY (ID_Nhom) REFERENCES NHOMNGUOIDUNG(ID_Nhom),
    FOREIGN KEY (ID_ChucNang) REFERENCES CHUCNANG(ID_ChucNang)
);

CREATE TABLE NHANVIEN (
    ID_NhanVien INT PRIMARY KEY IDENTITY(1,1),
    HoTenNV NVARCHAR(500),
    NgaySinh DATE,
    GioiTinh NVARCHAR(10),
    CCCD NVARCHAR(20) UNIQUE,
    DienThoai NVARCHAR(15) UNIQUE,
    DiaChi NVARCHAR(500),
    Email NVARCHAR(500) UNIQUE,
    HinhAnh NVARCHAR(255),
	TrangThai NVARCHAR(50) DEFAULT N'Đang làm việc',
    ID_Nhom INT,
	MatKhau NVARCHAR(50),
    FOREIGN KEY (ID_Nhom) REFERENCES NHOMNGUOIDUNG(ID_Nhom)
);

CREATE TABLE BENHNHAN (
    ID_BenhNhan INT PRIMARY KEY IDENTITY(1,1),
    HoTenBN NVARCHAR(500) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10) NOT NULL,
    CCCD NVARCHAR(20) UNIQUE,
    DienThoai NVARCHAR(15) UNIQUE NOT NULL,
	DiaChi NVARCHAR(500),
    Email NVARCHAR(100) UNIQUE,
	Is_Deleted BIT DEFAULT 0,
	NgayDK DATE,
);

CREATE TABLE DANHSACHTIEPNHAN (
	ID_TiepNhan INT PRIMARY KEY IDENTITY(1,1),
    ID_BenhNhan INT NOT NULL,
    NgayTN DATETIME NOT NULL,
	CaTN NVARCHAR(10) NOT NULL,
    ID_NhanVien INT NOT NULL,
	Is_Deleted BIT DEFAULT 0,
	TrangThai BIT DEFAULT 0,
    FOREIGN KEY (ID_BenhNhan) REFERENCES BENHNHAN(ID_BenhNhan),
    FOREIGN KEY (ID_NhanVien) REFERENCES NHANVIEN(ID_NhanVien)
);

CREATE TABLE LOAIBENH (
    ID_LoaiBenh INT PRIMARY KEY IDENTITY(1,1),
    TenLoaiBenh NVARCHAR(100) NOT NULL,
    TrieuChung NVARCHAR(255),
    HuongDieuTri NVARCHAR(255)
);

CREATE TABLE PHIEUKHAM (
    ID_PhieuKham INT PRIMARY KEY IDENTITY(1,1),
    ID_TiepNhan INT NOT NULL,
    CAKham NVARCHAR(10) NOT NULL,
    TrieuChung NVARCHAR(255),
    ID_LoaiBenh INT NOT NULL,
    TienKham DECIMAL(10,2),
    TongTienThuoc DECIMAL(10,2) DEFAULT 0,
	Is_Deleted BIT DEFAULT 0,
    FOREIGN KEY (ID_TiepNhan) REFERENCES DANHSACHTIEPNHAN(ID_TiepNhan),
	FOREIGN KEY (ID_LoaiBenh) REFERENCES LOAIBENH(ID_LoaiBenh)
);

CREATE TABLE DVT (
    ID_DVT INT PRIMARY KEY IDENTITY(1,1),
    TenDVT NVARCHAR(50) NOT NULL
);

CREATE TABLE CACHDUNG (
    ID_CachDung INT PRIMARY KEY IDENTITY(1,1),
    MoTaCachDung NVARCHAR(100) NOT NULL
);


CREATE TABLE THUOC (
    ID_Thuoc INT PRIMARY KEY IDENTITY(1,1),
    TenThuoc NVARCHAR(100) NOT NULL,
    ID_DVT INT NOT NULL,
    ID_CachDung INT NOT NULL,
    ThanhPhan NVARCHAR(255),
    XuatXu NVARCHAR(100),
    SoLuongTon INT NOT NULL DEFAULT 0,
    DonGiaNhap DECIMAL(10,2),
    HinhAnh NVARCHAR(255),
    TyLeGiaBan DECIMAL(10,2),
	DonGiaBan DECIMAL(10,2) NULL,
	IsDeleted BIT DEFAULT 0,
    FOREIGN KEY (ID_DVT) REFERENCES DVT(ID_DVT),
    FOREIGN KEY (ID_CachDung) REFERENCES CACHDUNG(ID_CachDung)
);

CREATE TABLE TOATHUOC (
    ID_PhieuKham INT NOT NULL,
    ID_Thuoc INT NOT NULL,
    SoLuong INT NOT NULL,
	DonGiaBan_LucMua DECIMAL(10,2),
    TienThuoc DECIMAL(10,2) DEFAULT 0,
    PRIMARY KEY (ID_PhieuKham, ID_Thuoc),
    FOREIGN KEY (ID_PhieuKham) REFERENCES PHIEUKHAM(ID_PhieuKham),
    FOREIGN KEY (ID_Thuoc) REFERENCES THUOC(ID_Thuoc)
);

CREATE TABLE PHIEUNHAPTHUOC (
    ID_PhieuNhapThuoc INT PRIMARY KEY IDENTITY(1,1),
    NgayNhap DATE NOT NULL,
    TongTienNhap DECIMAL(10,2) DEFAULT 0
);

CREATE TABLE CHITIETPHIEUNHAPTHUOC (
    ID_PhieuNhapThuoc INT NOT NULL,
    ID_Thuoc INT NOT NULL,
	HanSuDung DATE, 
    SoLuongNhap INT NOT NULL,
    DonGiaNhap DECIMAL(10,2) NOT NULL,
	ThanhTien DECIMAL(10,2) DEFAULT 0,
    PRIMARY KEY (ID_PhieuNhapThuoc, ID_Thuoc),
    FOREIGN KEY (ID_PhieuNhapThuoc) REFERENCES PHIEUNHAPTHUOC(ID_PhieuNhapThuoc),
    FOREIGN KEY (ID_Thuoc) REFERENCES THUOC(ID_Thuoc)
);

CREATE TABLE HOADON (
    ID_HoaDon INT PRIMARY KEY IDENTITY(1,1),
    ID_PhieuKham INT NOT NULL,
    ID_NhanVien INT NOT NULL,
    NgayHoaDon DATE NOT NULL,
	TienKham DECIMAL(10,2),
	TienThuoc DECIMAL(10,2),
    TongTien DECIMAL(10,2) DEFAULT 0,
    FOREIGN KEY (ID_PhieuKham) REFERENCES PHIEUKHAM(ID_PhieuKham),
    FOREIGN KEY (ID_NhanVien) REFERENCES NHANVIEN(ID_NhanVien)
);

CREATE TABLE BAOCAODOANHTHU (
    ID_BCDT INT PRIMARY KEY IDENTITY(1,1),
    Thang INT NOT NULL,
    Nam INT NOT NULL,
	TongDoanhThu DECIMAL(10,2),
);

CREATE TABLE CT_BAOCAODOANHTHU(
	ID_CTBCDT INT PRIMARY KEY IDENTITY(1,1),
    Ngay INT NOT NULL,
    SoBenhNhan INT NOT NULL,
    DoanhThu DECIMAL(10,2) NOT NULL,
    TyLe DECIMAL(5,2) NOT NULL,
	ID_BCDT INT NOT NULL,
	FOREIGN KEY (ID_BCDT) REFERENCES BAOCAODOANHTHU(ID_BCDT)
);

CREATE TABLE BAOCAOSUDUNGTHUOC (
    ID_BCSDT INT PRIMARY KEY IDENTITY(1,1),
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    ID_Thuoc INT NOT NULL,
    TongSoLuongNhap INT NOT NULL,
	SoLuongDung INT NOT NULL,
    SoLanDung INT NOT NULL,
    FOREIGN KEY (ID_Thuoc) REFERENCES THUOC(ID_Thuoc)
);
------------------------------------------------------------------------


----------------------------QUI DINH, THAM SO----------------------------
CREATE TABLE QUI_DINH (
    TenQuiDinh NVARCHAR(100)PRIMARY KEY NOT NULL,
    GiaTri  DECIMAL(10,2) NOT NULL
);

INSERT INTO QUI_DINH (TenQuiDinh, GiaTri) VALUES 
('SoLuongTiepNhanToiDa', 40),
('TienKham', 30000),
('TyLeDonGiaBan', 1.30);

CREATE OR ALTER TRIGGER TRG_CheckSoLuongTiepNhan
ON DANHSACHTIEPNHAN
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @NgayTN DATE, @Count INT, @Limit INT

    -- Lấy ngày tiếp nhận từ bản ghi đang được chèn
    SELECT TOP 1 @NgayTN = CAST(NgayTN AS DATE) FROM inserted

    -- Đếm số lượng tiếp nhận trong ngày đó
    SELECT @Count = COUNT(*)
    FROM DANHSACHTIEPNHAN
    WHERE CAST(NgayTN AS DATE) = @NgayTN

    -- Lấy giới hạn từ bảng QUI_DINH
    SELECT @Limit = GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'SoLuongTiepNhanToiDa'

    -- Kiểm tra vượt giới hạn
    IF @Count >= @Limit
    BEGIN
        RAISERROR(N'Vượt quá số lượng tiếp nhận tối đa trong ngày', 16, 1)
        ROLLBACK TRANSACTION
    END
    ELSE
        INSERT INTO DANHSACHTIEPNHAN (ID_BenhNhan, NgayTN, CaTN, ID_NhanVien, Is_Deleted, TrangThai)
        SELECT ID_BenhNhan, NgayTN, CaTN, ID_NhanVien, Is_Deleted, TrangThai FROM inserted
END

CREATE OR ALTER TRIGGER TRG_SetTienKham_FromQuiDinh
ON PHIEUKHAM
AFTER INSERT
AS
BEGIN
    DECLARE @TienKham DECIMAL(10,2)
    SELECT @TienKham = GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'TienKham'

    UPDATE PK
    SET TienKham = @TienKham
    FROM PHIEUKHAM PK
    JOIN inserted I ON PK.ID_PhieuKham = I.ID_PhieuKham
END

CREATE OR ALTER TRIGGER TRG_SetTyLeGiaBan_Default
ON THUOC
AFTER INSERT
AS
BEGIN
    DECLARE @TyLeMacDinh DECIMAL(10,2)
    SELECT @TyLeMacDinh = GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'TyLeDonGiaBan'

    -- Cập nhật tất cả thuốc vừa insert có TyLeGiaBan là NULL
    UPDATE T
    SET T.TyLeGiaBan = @TyLeMacDinh
    FROM THUOC T
    INNER JOIN inserted i ON T.ID_Thuoc = i.ID_Thuoc
    WHERE T.TyLeGiaBan IS NULL
END

------------------------------------------------------------------------


---------------------------THUOC TINH TU TINH---------------------------
CREATE OR ALTER PROC proc_login
	@user NVARCHAR(50),
	@pass NVARCHAR(50)
AS
BEGIN
	SELECT NV.Email, NV.MatKhau, NV.ID_Nhom
	FROM NHANVIEN NV
	INNER JOIN NHOMNGUOIDUNG NND ON NV.ID_Nhom = NND.ID_Nhom
	WHERE NV.Email = @user AND NV.MatKhau = @pass;
END;

CREATE OR ALTER TRIGGER trg_CalculateDonGiaBan
ON THUOC
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE THUOC
    SET DonGiaBan = DonGiaNhap * TyLeGiaBan
    WHERE ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;

CREATE OR ALTER TRIGGER trg_CalculateTienThuoc
ON TOATHUOC
AFTER INSERT
AS
BEGIN
    -- Khi thêm toa thuốc mới, lưu luôn giá bán vào thời điểm đó
    UPDATE TOATHUOC
    SET DonGiaBan_LucMua = th.DonGiaBan,
        TienThuoc = t.SoLuong * th.DonGiaBan
    FROM TOATHUOC t
    JOIN THUOC th ON t.ID_Thuoc = th.ID_Thuoc
    WHERE t.ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;


CREATE OR ALTER TRIGGER trg_UpdateTongTienThuoc
ON TOATHUOC
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE PHIEUKHAM
    SET TongTienThuoc = (
        SELECT COALESCE(SUM(TienThuoc), 0)
        FROM TOATHUOC
        WHERE TOATHUOC.ID_PhieuKham = PHIEUKHAM.ID_PhieuKham
    )
    WHERE ID_PhieuKham IN (
        SELECT ID_PhieuKham FROM inserted
        UNION 
        SELECT ID_PhieuKham FROM deleted
    );
END;

CREATE OR ALTER TRIGGER trg_CalculateThanhTien
ON CHITIETPHIEUNHAPTHUOC
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE CHITIETPHIEUNHAPTHUOC
    SET ThanhTien = SoLuongNhap * DonGiaNhap
    WHERE ID_PhieuNhapThuoc IN (SELECT ID_PhieuNhapThuoc FROM inserted)
          AND ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;

CREATE TRIGGER trg_UpdateTongTienNhap
ON CHITIETPHIEUNHAPTHUOC
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    UPDATE PHIEUNHAPTHUOC
    SET TongTienNhap = (
        SELECT COALESCE(SUM(ThanhTien), 0)
        FROM CHITIETPHIEUNHAPTHUOC
        WHERE CHITIETPHIEUNHAPTHUOC.ID_PhieuNhapThuoc = PHIEUNHAPTHUOC.ID_PhieuNhapThuoc
    )
    WHERE ID_PhieuNhapThuoc IN (
        SELECT ID_PhieuNhapThuoc FROM inserted
        UNION
        SELECT ID_PhieuNhapThuoc FROM deleted
    );
END;

CREATE OR ALTER TRIGGER trg_UpdateTonKho_KhiNhapThuoc
ON CHITIETPHIEUNHAPTHUOC
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Khi UPDATE: Phải tính sự chênh lệch giữa giá trị cũ và mới
    IF EXISTS (SELECT * FROM deleted)
    BEGIN
        UPDATE T
        SET SoLuongTon = SoLuongTon + (I.SoLuongNhap - D.SoLuongNhap)
        FROM THUOC T
        JOIN inserted I ON T.ID_Thuoc = I.ID_Thuoc
        JOIN deleted D ON I.ID_PhieuNhapThuoc = D.ID_PhieuNhapThuoc AND I.ID_Thuoc = D.ID_Thuoc;
    END
    ELSE
    BEGIN
        -- Khi INSERT mới: chỉ cộng thêm
        UPDATE T
        SET SoLuongTon = SoLuongTon + I.SoLuongNhap
        FROM THUOC T
        JOIN inserted I ON T.ID_Thuoc = I.ID_Thuoc;
    END

    -- Luôn cập nhật DonGiaNhap mới nhất
    UPDATE T
    SET DonGiaNhap = I.DonGiaNhap
    FROM THUOC T
    JOIN inserted I ON T.ID_Thuoc = I.ID_Thuoc;
END


CREATE OR ALTER TRIGGER trg_UpdateTonKho_KhiBanThuoc
ON TOATHUOC
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @thuoc TABLE (ID_Thuoc INT, Delta INT);

    -- Nếu có bản ghi bị xóa (DELETE hoặc UPDATE): trừ ngược lại
    INSERT INTO @thuoc(ID_Thuoc, Delta)
    SELECT ID_Thuoc, SoLuong * (-1) FROM deleted;

    -- Nếu có bản ghi được thêm (INSERT hoặc UPDATE): trừ vào tồn kho
    INSERT INTO @thuoc(ID_Thuoc, Delta)
    SELECT ID_Thuoc, SoLuong FROM inserted;

    -- Cộng dồn Delta để cập nhật tồn kho
    UPDATE T
    SET SoLuongTon = SoLuongTon - A.TotalBan
    FROM THUOC T
    JOIN (
        SELECT ID_Thuoc, SUM(Delta) AS TotalBan
        FROM @thuoc
        GROUP BY ID_Thuoc
    ) A ON T.ID_Thuoc = A.ID_Thuoc;
END;




CREATE OR ALTER TRIGGER TRG_AutoFill_HoaDon
ON HOADON
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    -- Cập nhật TienKham, TienThuoc, TongTien dựa trên dữ liệu từ PHIEUKHAM
    UPDATE H
    SET 
        H.TienKham = PK.TienKham,
        H.TienThuoc = PK.TongTienThuoc,
        H.TongTien = ISNULL(PK.TienKham, 0) + ISNULL(PK.TongTienThuoc, 0)
    FROM HOADON H
    INNER JOIN inserted I ON H.ID_HoaDon = I.ID_HoaDon
    INNER JOIN PHIEUKHAM PK ON I.ID_PhieuKham = PK.ID_PhieuKham
END




CREATE OR ALTER PROCEDURE TaoBaoCaoDoanhThu
    @Thang INT,
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    -- Tổng doanh thu trong tháng
    DECLARE @TongDoanhThu DECIMAL(10,2)

    SELECT @TongDoanhThu = SUM(TongTien)
    FROM HOADON
    WHERE MONTH(NgayHoaDon) = @Thang AND YEAR(NgayHoaDon) = @Nam

    -- Thêm vào bảng BAOCAODOANHTHU
    INSERT INTO BAOCAODOANHTHU (Thang, Nam, TongDoanhThu)
    VALUES (@Thang, @Nam, @TongDoanhThu)

    DECLARE @ID_BCDT INT = SCOPE_IDENTITY()

    -- Chi tiết doanh thu theo ngày
    INSERT INTO CT_BAOCAODOANHTHU (Ngay, SoBenhNhan, DoanhThu, TyLe, ID_BCDT)
    SELECT 
        DAY(NgayHoaDon) AS Ngay,
        COUNT(*) AS SoBenhNhan,
        SUM(TongTien) AS DoanhThu,
        ROUND(SUM(TongTien) * 100.0 / NULLIF(@TongDoanhThu, 0), 2) AS TyLe,
        @ID_BCDT
    FROM HOADON
    WHERE MONTH(NgayHoaDon) = @Thang AND YEAR(NgayHoaDon) = @Nam
    GROUP BY DAY(NgayHoaDon)
END


CREATE OR ALTER PROCEDURE TaoBaoCaoSuDungThuoc
    @Thang INT,
    @Nam INT
AS
BEGIN
    SET NOCOUNT ON;

    -- CTE để tính tổng số lượng nhập theo thuốc trong tháng/năm
    ;WITH Nhap AS (
        SELECT ID_Thuoc, SUM(SoLuongNhap) AS TongNhap
        FROM CHITIETPHIEUNHAPTHUOC
        WHERE MONTH((SELECT NgayNhap FROM PHIEUNHAPTHUOC WHERE ID_PhieuNhapThuoc = CHITIETPHIEUNHAPTHUOC.ID_PhieuNhapThuoc)) = @Thang
          AND YEAR((SELECT NgayNhap FROM PHIEUNHAPTHUOC WHERE ID_PhieuNhapThuoc = CHITIETPHIEUNHAPTHUOC.ID_PhieuNhapThuoc)) = @Nam
        GROUP BY ID_Thuoc
    )

    INSERT INTO BAOCAOSUDUNGTHUOC (Thang, Nam, ID_Thuoc, TongSoLuongNhap, SoLuongDung, SoLanDung)
    SELECT 
        @Thang,
        @Nam,
        T.ID_Thuoc,
        ISNULL(N.TongNhap, 0) AS TongSoLuongNhap, -- Tổng số lượng nhập
        SUM(ISNULL(T.SoLuong, 0)), -- Đã dùng
        COUNT(*) -- Số lần xuất hiện
    FROM TOATHUOC T
    INNER JOIN PHIEUKHAM PK ON T.ID_PhieuKham = PK.ID_PhieuKham
    INNER JOIN HOADON HD ON PK.ID_PhieuKham = HD.ID_PhieuKham
    LEFT JOIN Nhap N ON N.ID_Thuoc = T.ID_Thuoc
    WHERE MONTH(HD.NgayHoaDon) = @Thang AND YEAR(HD.NgayHoaDon) = @Nam
    GROUP BY T.ID_Thuoc, N.TongNhap
END


------------------------------------------------------------------------


-----------------------------INSERT DU LIEU-----------------------------
INSERT INTO DVT (TenDVT) VALUES 
(N'Viên'),
(N'Chai');

INSERT INTO CACHDUNG (MoTaCachDung) VALUES 
(N'Uống sau ăn'),
(N'Uống trước ăn'),
(N'Ngậm dưới lưỡi'),
(N'Tiêm bắp');

INSERT INTO LOAIBENH (TenLoaiBenh, TrieuChung, HuongDieuTri) VALUES 
(N'Cảm cúm', N'Sốt, đau đầu, sổ mũi', N'Uống thuốc hạ sốt, nghỉ ngơi'),
(N'Tiêu chảy', N'Đi ngoài nhiều lần, mất nước', N'Bù nước, uống men tiêu hóa'),
(N'Viêm họng', N'Đau họng, ho, sốt', N'Súc miệng nước muối, dùng kháng sinh nếu cần'),
(N'Đau dạ dày', N'Đau bụng, ợ chua, khó tiêu', N'Dùng thuốc giảm tiết axit, ăn uống hợp lý'),
(N'Cao huyết áp', N'Huyết áp cao, chóng mặt', N'Thay đổi chế độ ăn uống, dùng thuốc hạ áp');


INSERT INTO THUOC (TenThuoc, ID_DVT, ID_CachDung, ThanhPhan, XuatXu, SoLuongTon, DonGiaNhap, HinhAnh)
VALUES 
(N'Paracetamol', 1, 1, N'Paracetamol 500mg', N'Việt Nam', 100, 1000, N'D:\THUOC\paracetamol_500mg.jpg'),
(N'Ibuprofen', 1, 1, N'Ibuprofen 200mg', N'Mỹ', 50, 2000, N'D:\THUOC\ibuprofen.jpg'),
(N'Amoxicillin', 1, 1, N'Amoxicillin 500mg', N'Pháp', 80, 1500, N'D:\THUOC\amoxicillin.jpg'),
(N'Cefuroxime', 1, 1, N'Cefuroxime 250mg', N'Anh', 40, 3000, N'D:\THUOC\cefuroxime.jpg'),
(N'Vitamin C', 1, 1, N'Acid ascorbic 500mg', N'Việt Nam', 120, 800, N'D:\THUOC\vitaminC.jpg'),
(N'ORS', 2, 2, N'Bù nước và điện giải', N'Việt Nam', 60, 5000, N'D:\THUOC\ors.jpg'),
(N'Oresol', 2, 2, N'Bù nước và điện giải', N'Mỹ', 30, 6000, N'D:\THUOC\oresol.jpg'),
(N'Azithromycin', 1, 1, N'Azithromycin 250mg', N'Ấn Độ', 70, 2500, N'D:\THUOC\azithromycin.jpg'),
(N'Clarithromycin', 1, 1, N'Clarithromycin 500mg', N'Nhật', 35, 4000, N'D:\THUOC\clarithromycin.jpg'),
(N'Ranitidine', 1, 1, N'Ranitidine 150mg', N'Việt Nam', 90, 1200, N'D:\THUOC\ranitidine.jpg'),
(N'Omeprazole', 1, 1, N'Omeprazole 20mg', N'Đức', 100, 1800, N'D:\THUOC\omeprazole.jpg'),
(N'Lansoprazole', 1, 1, N'Lansoprazole 30mg', N'Mỹ', 60, 2000, N'D:\THUOC\lansoprazol.jpg'),
(N'Losartan', 1, 1, N'Losartan 50mg', N'Anh', 80, 2500, N'D:\THUOC\losartan.jpg'),
(N'Amlodipine', 1, 1, N'Amlodipine 5mg', N'Nhật', 55, 2200, N'D:\THUOC\amlodipin.jpg'),
(N'Atenolol', 1, 1, N'Atenolol 50mg', N'Pháp', 40, 2100, N'D:\THUOC\Atenolol.jpg'),
(N'Glucose 5%', 2, 4, N'Dung dịch Glucose 5%', N'Việt Nam', 50, 7000, N'D:\THUOC\Glucose.jpg'),
(N'Natri clorid 0.9%', 2, 4, N'Dung dịch muối sinh lý', N'Việt Nam', 60, 5000, N'D:\THUOC\Natri_clorid.jpg'),
(N'Bromhexin', 1, 1, N'Bromhexin 8mg', N'Đức', 30, 1800, N'D:\THUOC\Bromhexin.jpg'),
(N'Dextromethorphan', 1, 1, N'Dextromethorphan 15mg', N'Việt Nam', 70, 1200, N'D:\THUOC\Dextromethorphan.jpg'),
(N'Codein', 1, 1, N'Codein 10mg', N'Mỹ', 50, 2500, N'D:\THUOC\Codein.jpg'),
(N'Levocetirizine', 1, 1, N'Levocetirizine 5mg', N'Anh', 80, 1400, N'D:\THUOC\Levocetirizine.png'),
(N'Loratadine', 1, 1, N'Loratadine 10mg', N'Nhật', 60, 1600, N'D:\THUOC\Loratadine.jpg'),
(N'Cetirizine', 1, 1, N'Cetirizine 10mg', N'Pháp', 50, 1700, N'D:\THUOC\Cetirizine.png'),
(N'Prednisolone', 1, 1, N'Prednisolone 5mg', N'Việt Nam', 40, 1300, N'D:\THUOC\Prednisolone.jpg'),
(N'Dexamethasone', 1, 1, N'Dexamethasone 0.5mg', N'Đức', 30, 1100, N'D:\THUOC\Dexamethasone.jpg'),
(N'Insulin', 2, 4, N'Insulin tiêm', N'Mỹ', 20, 50000, N'D:\THUOC\Insulin.png'),
(N'Adrenaline', 2, 4, N'Adrenaline tiêm', N'Anh', 25, 60000, N'D:\THUOC\Adrenaline.png'),
(N'Atropin', 2, 4, N'Atropin tiêm', N'Pháp', 30, 70000, N'D:\THUOC\Atropin.jpg'),
(N'Furosemide', 1, 1, N'Furosemide 40mg', N'Việt Nam', 50, 1900, N'D:\THUOC\Furosemide.jpg'),
(N'Aspirin', 1, 2, N'Acetylsalicylic Acid', N'Việt Nam', 100, 5000, N'D:\THUOC\Aspirin.jpg');

INSERT INTO PHIEUNHAPTHUOC (NgayNhap)
VALUES 
('2025-03-30'),
('2025-03-31');

INSERT INTO CHITIETPHIEUNHAPTHUOC (ID_PhieuNhapThuoc, ID_Thuoc,HanSuDung, SoLuongNhap, DonGiaNhap)
VALUES 
(1, 1,'2027-01-01', 50, 2000), -- Paracetamol
(1, 2,'2027-01-01', 30, 2000), -- Ibuprofen
(1, 3,'2027-01-01', 40, 1500), -- Amoxicillin
(1, 4,'2027-01-01', 20, 3000), -- Cefuroxime
(2, 5,'2027-01-01', 100, 800), -- Vitamin C
(2, 6,'2027-01-01', 10, 5000), -- ORS
(2, 7,'2027-01-01', 15, 6000), -- Oresol
(2, 8,'2027-01-01', 25, 2500); -- Azithromycin


INSERT INTO NHOMNGUOIDUNG(TenNhom) 
VALUES (N'Admin'), (N'Thu ngân'), (N'Nhân viên lễ tân');

INSERT INTO NHANVIEN (HoTenNV, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, HinhAnh, ID_Nhom, MatKhau)
VALUES 
(N'Phạm Thị Hồng Hạnh', '2005-01-01', N'Nữ', '123456789101', '0987654321', N'123 Đường ABC, TP.HCM', '23520442@gm.uit.edu.vn', N'D:\IMAGE\nhanvien1.png', 3,'123456'),
(N'Nguyễn Phương Nam', '2005-01-01', N'Nam', '123456789102', '0987654322', N'234 Đường BCD, TPHCM', '23520980@gm.uit.edu.vn', N'D:\IMAGE\nhanvien2.png', 3, '123456'),
(N'Phạm Thị Kiều Diễm', '2005-06-09', N'Nữ', '123456789103', '0987654323', N'345 Đường CDE, TP.HCM', '23520286@gm.uit.edu.vn', N'D:\IMAGE\nhanvien3.png', 2, '123456'),
(N'Nguyễn Trương Ngọc Hân', '2005-04-19', N'Nữ', '123456789104', '0987654324', N'456 Đường DEF, TP.HCM', '23520434@gm.uit.edu.vn', N'D:\IMAGE\nhanvien4.png', 2, '123456');

INSERT INTO NHANVIEN (HoTenNV, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, HinhAnh, ID_Nhom, MatKhau)
VALUES 
(N'Trần Thị Bích Là', '2005-10-18', N'Nữ', '123456789105', '0333429390', N'123 Đường ABC, TP.HCM', '23520827@gm.uit.edu.vn', N'D:\IMAGE\nhanvien1.png', 1,'123456')

INSERT INTO BENHNHAN (HoTenBN, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, NgayDK)  
VALUES  
(N'Nuyễn Văn A', '1990-05-12', N'Nam', '123456789012', '0901234567', N'HCM', 'a@gmail.com', '2025-03-28'),
(N'Trần Thị B', '1985-07-20', N'Nữ', '223456789012', '0912345678', N'Hà Nội', 'b@gmail.com','2025-03-28'),
(N'Lê Văn C', '1992-03-25', N'Nam', '323456789012', '0923456789', N'Đà Nẵng', 'c@gmail.com','2025-03-28'),
(N'Phạm Văn D', '1988-01-14', N'Nam', '423456789012', '0934567890', N'Hải Phòng', 'd@gmail.com','2025-03-28'),
(N'Hoàng Thị E', '1995-09-10', N'Nữ', '523456789012', '0945678901', N'Cần Thơ', 'e@gmail.com','2025-03-28'),
(N'Bùi Văn F', '1993-07-07', N'Nam', '623456789012', '0956789012', N'Nha Trang', 'f@gmail.com','2025-03-28'),
(N'Đỗ Thị G', '1998-02-17', N'Nữ', '723456789012', '0967890123', N'Huế', 'g@gmail.com','2025-03-28'),
(N'Nguyễn Văn H', '2000-06-05', N'Nam', '823456789012', '0978901234', N'Đồng Nai', 'h@gmail.com','2025-03-28'),
(N'Phạm Thị I', '1987-11-23', N'Nữ', '923456789012', '0989012345', N'Vũng Tàu', 'i@gmail.com','2025-03-28'),
(N'Trần Văn J', '1991-04-12', N'Nam', '113456789012', '0990123456', N'Tây Ninh', 'j@gmail.com','2025-03-28'),
(N'Hoàng Vân K', '1986-03-08', N'Nam', '213456789012', '0901111222', N'Quảng Ninh', 'k@gmail.com','2025-03-28'),
(N'Nguyễn Thị L', '1994-08-18', N'Nữ', '313456789012', '0912222333', N'Lai Châu', 'l@gmail.com','2025-03-28'),
(N'Phạm Văn M', '1999-05-29', N'Nam', '413456789012', '0923333444', N'Lạng Sơn', 'm@gmail.com','2025-03-28'),
(N'Đỗ Thị N', '1997-07-14', N'Nữ', '513456789012', '0934444555', N'Hà Tĩnh', 'n@gmail.com','2025-03-28'),
(N'Bùi Văn O', '1996-01-21', N'Nam', '613456789012', '0945555666', N'Nam Định', 'o@gmail.com','2025-03-28'),
(N'Lê Thị P', '1984-10-10', N'Nữ', '713456789012', '0956666777', N'Vĩnh Phúc', 'p@gmail.com','2025-03-28'),
(N'Nguyễn Văn Q', '1983-12-30', N'Nam', '813456789012', '0967777888', N'Bạc Liêu', 'q@gmail.com','2025-03-28'),
(N'Trần Thị R', '2001-02-05', N'Nữ', '913456789012', '0978888999', N'Bình Phước', 'r@gmail.com','2025-03-28'),
(N'Hoàng Văn S', '1990-09-09', N'Nam', '103456789012', '0989999000', N'Sóc Trăng', 's@gmail.com','2025-03-28'),
(N'Phạm Thị T', '2002-06-23', N'Nữ', '203456789012', '0990000111', N'Kiên Giang', 't@gmail.com','2025-03-28'),
(N'Nguyễn Văn U', '1995-04-04', N'Nam', '303456789012', '0901111223', N'Phú Yên', 'u@gmail.com','2025-03-28'),
(N'Trần Thị V', '1989-07-15', N'Nữ', '403456789012', '0912222334', N'Gia Lai', 'v@gmail.com','2025-03-28'),
(N'Hoàng Văn W', '1993-11-28', N'Nam', '503456789012', '0923333445', N'Dăk Lăk', 'w@gmail.com','2025-03-28'),
(N'Phạm Thị X', '1982-08-08', N'Nữ', '603456789012', '0934444556', N'Kon Tum', 'x@gmail.com','2025-03-28'),
(N'Đỗ Văn Y', '2003-03-19', N'Nam', '703456789012', '0945555667', N'Nghệ An', 'y@gmail.com','2025-03-28'),
(N'Bùi Thị Z', '1981-09-22', N'Nữ', '803456789012', '0956666778', N'Quảng Trị', 'z@gmail.com','2025-03-28'),
(N'Nguyễn Vân AA', '1998-12-07', N'Nam', '903456789012', '0967777889', N'Bến Tre', 'aa@gmail.com','2025-03-28'),
(N'Tran Thi BB', '1990-01-16', N'Nữ', '113456789013', '0978888990', N'Quảng Nam', 'bb@gmail.com','2025-03-28'),
(N'Hoàng Vân CC', '1985-06-25', N'Nam', '213456789013', '0989999001', N'Tuyên Quang', 'cc@gmail.com','2025-03-28'),
(N'Phạm Thị DD', '1994-11-11', N'Nữ', '313456789013', '0990000112', N'Cà Mau', 'dd@gmail.com','2025-03-28'),
(N'Nguyễn Văn EE', '1999-02-14', N'Nam', '413456789013', '0901111224', N'Bình Định', 'ee@gmail.com','2025-03-28'),
(N'Trần Thị FF', '1988-05-20', N'Nữ', '513456789013', '0912222335', N'Thái Nguyên', 'ff@gmail.com','2025-03-28'),
(N'Hoàng Văn GG', '1992-07-18', N'Nam', '613456789013', '0923333446', N'Bắc Giang', 'gg@gmail.com','2025-03-28'),
(N'Phạm Thị HH', '1987-03-29', N'Nữ', '713456789013', '0934444557', N'An Giang', 'hh@gmail.com','2025-03-28'),
(N'Lê Văn II', '2000-09-15', N'Nam', '813456789013', '0945555668', N'Lào Cai', 'ii@gmail.com','2025-03-28'),
(N'Đỗ Thị JJ', '1991-10-30', N'Nữ', '913456789013', '0956666779', N'Hà Giang', 'jj@gmail.com','2025-03-28'),
(N'Bùi Văn KK', '1986-12-19', N'Nam', '103456789013', '0967777890', N'Hậu Giang', 'kk@gmail.com','2025-03-28'),
(N'Phạm Thị LL', '1997-01-05', N'Nữ', '203456789013', '0978888901', N'Đồng Tháp', 'll@gmail.com','2025-03-28'),
(N'Phạm Thị Z', '2000-10-15', N'Nữ', '113456789112', '0999999999', N'Cần Thơ', 'zz@gmail.com','2025-03-28'),
(N'Nguyễn Văn An', '1990-05-10', N'Nam', '122456789012', '0992345678', N'Hà Nội', 'an.nguyen@example.com','2025-03-28'),
(N'Trần Thị Bích', '1985-08-22', N'Nữ', '987654321098', '0909654321', N'TP.HCM', 'bich.tran@example.com','2025-03-28'),
(N'Lê Hoàng Dũng', '2000-02-15', N'Nam', '192837465012', '0909123456', N'Đà Nẵng', 'dung.le@example.com','2025-03-28'),
(N'Phạm Thùy Linh', '1995-11-03', N'Nữ', '564738291045', '0911223344', N'Hải Phòng', 'linh.pham@example.com','2025-03-28'),
(N'Võ Minh Trí', '1988-07-19', N'Nam', '748392615087', '0977885566', N'Cần Thơ', 'tri.vo@example.com','2025-03-28'),
(N'Đặng Thị Hương', '1993-09-25', N'Nữ', '908172635049', '0966998877', N'Nghệ An', 'huong.dang@example.com','2025-03-28'),
(N'Bùi Văn Cường', '1982-04-12', N'Nam', '627394105827', '0955667788', N'Quảng Ninh', 'cuong.bui@example.com','2025-03-28'),
(N'Ngô Thanh Tuyền', '1998-06-30', N'Nữ', '382910574683', '0944556677', N'Bình Dương', 'tuyen.ngo@example.com','2025-03-28'),
(N'Hoàng Văn Nam', '1979-12-05', N'Nam', '271093847562', '0933445566', N'Bắc Giang', 'nam.hoang@example.com','2025-03-28'),
(N'Lý Thị Mai', '2001-03-18', N'Nữ', '472819503746', '0922334455', N'Huế', 'mai.ly@example.com','2025-03-28');

INSERT INTO DANHSACHTIEPNHAN (ID_BenhNhan, NgayTN, CaTN, ID_NhanVien)
VALUES 
(1, '2025-03-29', N'Sáng', 1),
(2, '2025-03-29', N'Chiều', 2),
(3, '2025-03-29', N'Sáng', 1),
(4, '2025-03-29', N'Chiều', 2),
(5, '2025-03-29', N'Sáng', 1),
(6, '2025-03-29', N'Chiều', 2),
(7, '2025-03-29', N'Sáng', 1),
(8, '2025-03-29', N'Chiều', 2),
(9, '2025-03-29', N'Sáng', 1),
(10, '2025-03-29', N'Chiều', 2),
(11, '2025-03-29', N'Sáng', 1),
(12, '2025-03-29', N'Chiều', 2),
(13, '2025-03-29', N'Sáng', 1),
(14, '2025-03-29', N'Chiều', 2),
(15, '2025-03-29', N'Sáng', 1),
(16, '2025-03-29', N'Chiều', 2),
(17, '2025-03-29', N'Sáng', 1),
(18, '2025-03-29', N'Chiều', 2),
(19, '2025-03-29', N'Sáng', 1),
(20, '2025-03-29', N'Chiều', 2),
(21, '2025-03-29', N'Sáng', 1),
(22, '2025-03-29', N'Chiều', 2),
(23, '2025-03-29', N'Sáng', 1),
(24, '2025-03-29', N'Chiều', 2),
(25, '2025-03-29', N'Sáng', 1),
(26, '2025-03-29', N'Chiều', 2),
(27, '2025-03-29', N'Sáng', 1),
(28, '2025-03-29', N'Chiều', 2),
(29, '2025-03-29', N'Sáng', 1),
(30, '2025-03-29', N'Chiều', 2),
(31, '2025-03-29', N'Sáng', 1),
(32, '2025-03-29', N'Chiều', 2),
(33, '2025-03-29', N'Sáng', 1),
(34, '2025-03-29', N'Chiều', 2),
(35, '2025-03-29', N'Sáng', 1),
(36, '2025-03-29', N'Chiều', 2),
(37, '2025-03-29', N'Sáng', 1),
(38, '2025-03-29', N'Chiều', 2),
(39, '2025-03-29', N'Sáng', 1),
(40, '2025-03-29', N'Chiều', 2),
(41, '2025-03-30', N'Sáng', 1),
(42, '2025-03-30', N'Chiều', 2),
(43, '2025-03-30', N'Sáng', 1),
(44, '2025-03-30', N'Chiều', 2),
(45, '2025-03-30', N'Sáng', 1),
(46, '2025-03-30', N'Chiều', 2),
(47, '2025-03-30', N'Sáng', 1),
(48, '2025-03-30', N'Chiều', 2),
(49, '2025-03-30', N'Sáng', 1);


INSERT INTO PHIEUKHAM (ID_TiepNhan, CAKham, TrieuChung, ID_LoaiBenh)
VALUES
(1, N'Chiều', N'Sốt, đau đầu, sổ mũi', 1),
(2, N'Chiều', N'Đi ngoài nhiều lần, mất nước', 2),
(3, N'Chiều', N'Đau họng, ho, sốt', 3),
(4, N'Chiều', N'Đau bụng, ợ chua, khó tiêu', 4),
(5, N'Chiều', N'Huyết áp cao, chóng mặt', 5),
(6, N'Chiều', N'Sốt, đau đầu, sổ mũi', 1),
(7, N'Chiều', N'Đi ngoài nhiều lần, mất nước', 2),
(8, N'Chiều', N'Đau họng, ho, sốt', 3),
(9, N'Chiều', N'Đau bụng, ợ chua, khó tiêu', 4),
(10, N'Chiều', N'Huyết áp cao, chóng mặt', 5),
(11, N'Chiều', N'Sốt, đau đầu, sổ mũi', 1),
(12, N'Chiều', N'Đi ngoài nhiều lần, mất nước', 2),
(13, N'Chiều', N'Đau họng, ho, sốt', 3),
(14, N'Chiều', N'Đau bụng, ợ chua, khó tiêu', 4),
(15, N'Chiều', N'Huyết áp cao, chóng mặt', 5),
(16, N'Chiều', N'Sốt, đau đầu, sổ mũi', 1),
(17, N'Chiều', N'Đi ngoài nhiều lần, mất nước', 2),
(18, N'Chiều', N'Đau họng, ho, sốt', 3),
(19, N'Chiều', N'Đau bụng, ợ chua, khó tiêu', 4),
(20, N'Chiều', N'Huyết áp cao, chóng mặt', 5),
(21, N'Chiều', N'Sốt, đau đầu, sổ mũi', 1),
(22, N'Chiều', N'Đi ngoài nhiều lần, mất nước', 2),
(23, N'Chiều', N'Đau họng, ho, sốt', 3),
(24, N'Chiều', N'Đau bụng, ợ chua, khó tiêu', 4),
(25, N'Chiều', N'Huyết áp cao, chóng mặt', 5),
(26, N'Chiều', N'Sốt, đau đầu, sổ mũi', 1),
(27, N'Chiều', N'Đi ngoài nhiều lần, mất nước', 2),
(28, N'Chiều', N'Đau họng, ho, sốt', 3),
(29, N'Chiều', N'Đau bụng, ợ chua, khó tiêu', 4),
(30, N'Chiều', N'Huyết áp cao, chóng mặt', 5);
SELECT * FROM PHIEUKHAM

INSERT INTO TOATHUOC (ID_PhieuKham, ID_Thuoc, SoLuong)
VALUES
(1,  2,  3),  
(1,  5,  2),  
(2,  1,  4),  
(2,  3,  1),  
(3,  4,  5),  
(3,  2,  3),  
(4,  6,  2),  
(5,  3,  4),  
(5,  1,  3),  
(6,  2,  2),  
(7,  5,  5),  
(8,  4,  1),  
(8,  6,  3),  
(9,  3,  2),  
(10, 1,  4),  
(10, 5,  3),  
(11, 2,  5),  
(12, 3,  2),  
(13, 6,  1),  
(14, 4,  3),  
(15, 1,  2),  
(16, 5,  4),  
(17, 2,  1),  
(18, 6,  3),  
(19, 4,  5),  
(20, 3,  2),  
(21, 1,  4),  
(22, 2,  3),  
(23, 5,  1),  
(24, 6,  5),  
(25, 4,  2),  
(26, 1,  3),  
(27, 5,  4),  
(28, 2,  1),  
(29, 6,  3),  
(30, 4,  2);

INSERT INTO HOADON (ID_PhieuKham, ID_NhanVien, NgayHoaDon)
VALUES
(1, 3, '2025-03-29'),
(2, 4, '2025-03-29'),
(3, 3, '2025-03-29'),
(4, 4, '2025-03-29'),
(5, 3, '2025-03-29'),
(6, 4, '2025-03-29'),
(7, 3, '2025-03-29'),
(8, 4, '2025-03-29'),
(9, 3, '2025-03-29'),
(10, 4, '2025-03-29'),
(11, 3, '2025-03-29'),
(12, 4, '2025-03-29'),
(13, 3, '2025-03-29'),
(14, 4, '2025-03-29'),
(15, 3, '2025-03-29'),
(16, 4, '2025-03-29'),
(17, 3, '2025-03-29'),
(18, 4, '2025-03-29'),
(19, 3, '2025-03-29'),
(20, 4, '2025-03-29'),
(21, 3, '2025-03-29'),
(22, 4, '2025-03-29'),
(23, 3, '2025-03-29'),
(24, 4, '2025-03-29'),
(25, 3, '2025-03-29'),
(26, 4, '2025-03-29'),
(27, 3, '2025-03-29'),
(28, 4, '2025-03-29'),
(29, 3, '2025-03-29'),
(30, 4, '2025-03-29');



----------------------------TRUY VAN CAC BANG----------------------------
SELECT * FROM QUI_DINH
SELECT * FROM DVT
SELECT * FROM CACHDUNG
SELECT * FROM LOAIBENH
SELECT * FROM THUOC
SELECT * FROM PHIEUNHAPTHUOC
SELECT * FROM CHITIETPHIEUNHAPTHUOC
SELECT * FROM NHOMNGUOIDUNG
SELECT * FROM NHANVIEN
SELECT * FROM BENHNHAN
SELECT * FROM DANHSACHTIEPNHAN
SELECT * FROM PHIEUKHAM
SELECT * FROM TOATHUOC
SELECT * FROM HOADON
EXEC TaoBaoCaoDoanhThu @Thang = 6, @Nam = 2025;
EXEC TaoBaoCaoSuDungThuoc @Thang = 6, @Nam = 2025;
SELECT * FROM PHANQUYEN
SELECT * FROM CHUCNANG
SELECT * FROM NHOMNGUOIDUNG
SELECT * FROM BAOCAODOANHTHU
SELECT * FROM PHIEUNHAPTHUOC
SELECT * FROM CT_BAOCAODOANHTHU
SELECT * FROM BAOCAOSUDUNGTHUOC
SELECT * FROM QUI_DINH


---them tu day
INSERT INTO CHUCNANG (TenChucNang, TenManHinhDuocLoad) VALUES 
(N'Trang tổng quan', 'DashBoard'),
(N'Quản lý bệnh nhân', 'PatientList'),
(N'Danh sách tiếp nhận', 'ExaminationLis'),
(N'Quản lý kho thuốc', 'DrugView'),
(N'Quản lý nhập hàng', 'ReceiptList'),
(N'Trang thanh toán', 'CreateBill'),
(N'Quản lý hóa đơn', 'StaffAccount'),
(N'Trang báo cáo', 'ReportView'),
(N'Quản lý nhân viên', 'quan_ly_nhan_vien'),
(N'Thay đổi quy định', 'Setting'),
(N'Cài đặt phân quyền', 'RoleManagement'),

-- Logic-level
(N'Thêm bệnh nhân', 'btnAddPatient_Click'),
(N'Thêm phiếu tiếp nhận', 'btn_addPatientToExam_Click'),
(N'Thêm phiếu nhập thuốc', 'AddClick'),
(N'Thêm hóa đơn', 'CreateBill'),
(N'Sửa phiếu bệnh nhân', 'btn_editPatient_Click'),
(N'Sửa phiếu tiếp nhận', 'btn_editPatientFromExam_Click'),
(N'Sửa phiếu nhập thuốc', 'EditButton_Click'),
(N'Sửa hóa đơn', 'EditButton_Click'),
-- Xóa

(N'Xóa phiếu bệnh nhân', 'btn_deletePatientFromBenhNhan_Click'),
(N'Xóa phiếu tiếp nhận', 'btn_deletePatientFromExam_Click'),
(N'Xóa phiếu nhập thuốc', 'btnEdit_Click'),
(N'Xóa hóa đơn', 'DeleteBill_Click'),
-- Data-level
(N'Giới hạn theo dữ liệu', 'data');

INSERT INTO PHANQUYEN (ID_Nhom, ID_ChucNang) VALUES (2, 4); -- Thu ngân được quản lý kho thuốc
INSERT INTO PHANQUYEN (ID_Nhom, ID_ChucNang) VALUES (2, 5); -- Thu ngân được nhập hàng
INSERT INTO PHANQUYEN (ID_Nhom, ID_ChucNang) VALUES (1, 3);

DELETE FROM PHANQUYEN WHERE ID_Nhom IN (1, 3);

CREATE TABLE LOGINLOG (
    ID_Log INT IDENTITY(1,1) PRIMARY KEY,
    Email NVARCHAR(255) NOT NULL,
    ThoiGian DATETIME DEFAULT GETDATE(),
    TrangThai NVARCHAR(50) DEFAULT N'Đang làm việc', -- "Đang hoạt động" "Bị khóa"
    SoLanThatBai INT DEFAULT 0)
ALTER TABLE LOGINLOG
ADD HanhDong NVARCHAR(100) DEFAULT N'Đăng nhập';
SELECT * FROM LOGINLOG


--huongdansudungphanquyen
--1. Admin – Toàn quyền hệ thống
--Quản lý nhân sự, phân quyền, báo cáo, quy định, tổng quan

--STT	Tên chức năng
	--Trang tổng quan (DashBoard)
	--Trang báo cáo (ReportView)
	--Quản lý nhân viên (quan_ly_nhan_vien)
	--Cài đặt phân quyền (RoleManagement)
	--Thay đổi quy định (Setting)
	--Quản lý hóa đơn (StaffAccount)

-- Logic-level:

-- Sửa, xóa toàn bộ

-- Thêm toàn bộ

-- 2. Bác sĩ – Xử lý khám bệnh, tiếp nhận bệnh nhân
--STT	Tên chức năng
--	Quản lý bệnh nhân (PatientList)
--	Danh sách tiếp nhận (ExaminationLis)
-- Logic-level:

-- Thêm/Sửa phiếu bệnh nhân
-- Thêm/Sửa phiếu tiếp nhận

-- Không có quyền xóa, xuất thuốc, tạo hóa đơn

-- 3. Thu ngân – Quản lý thanh toán, hóa đơn
--STT	Tên chức năng
--	Trang thanh toán (CreateBill)
--	Quản lý hóa đơn (StaffAccount)

-- Logic-level:

-- Thêm/Sửa/Xóa hóa đơn

-- Không sửa dữ liệu khám, kho thuốc

-- 4. Kiểm kho – Nhập thuốc, kiểm kê
--STT	Tên chức năng
--	Quản lý kho thuốc (DrugView)
--	Quản lý nhập hàng (ReceiptList)

-- Logic-level:

-- Thêm/Sửa/Xóa phiếu nhập thuốc

-- 5. Tiếp tân – Tiếp nhận bệnh nhân, hỗ trợ bác sĩ
--STT	Tên chức năng
--	Trang tổng quan (DashBoard)
--	Danh sách tiếp nhận (ExaminationLis)

-- Logic-level:

-- Thêm phiếu tiếp nhận

 --Không sửa/xóa hay phân quyền

-- Không tạo hóa đơn hay nhập kho

-- Bổ sung: Chức năng Giới hạn theo dữ liệu (data)
--Gán cho Bác sĩ, Thu ngân, Kiểm kho, Tiếp tân

-- Admin không cần, vì có toàn quyền
--Chỉ log các sự kiện quan trọng, ví dụ:

--Truy cập các trang quản trị

--Truy cập chi tiết hồ sơ bệnh nhân

--Truy cập báo cáo tài chính

--Không log các trang trung gian hoặc lặp đi lặp lại (ví dụ: chuyển tab UI không quan trọng)

--Tùy biến theo phân quyền:

--Nếu là admin, log chi tiết

--Nếu là user thường, chỉ log những hành vi bất thường

--Dùng async hoặc batch logging nếu sợ ảnh hưởng hiệu năng



CREATE OR ALTER TRIGGER TRG_UpdateTyLeGiaBan_WhenRuleChanged
ON QUI_DINH
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;

    -- Chỉ khi cập nhật đúng dòng TyLeDonGiaBan
    IF EXISTS (
        SELECT * 
        FROM inserted i
        JOIN deleted d ON i.TenQuiDinh = d.TenQuiDinh
        WHERE i.TenQuiDinh = 'TyLeDonGiaBan'
          AND i.GiaTri != d.GiaTri
    )
    BEGIN
        DECLARE @NewTyLe DECIMAL(10,2);
        SELECT @NewTyLe = GiaTri FROM inserted WHERE TenQuiDinh = 'TyLeDonGiaBan';

        -- Cập nhật toàn bộ thuốc theo tỷ lệ mới
        UPDATE THUOC
        SET TyLeGiaBan = @NewTyLe;
    END
END