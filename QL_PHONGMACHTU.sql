CREATE DATABASE QL_PHONGMACHTU
GO

USE QL_PHONGMACHTU
GO

--------------------------CREATE TABLE--------------------------
CREATE TABLE VAITRO (
    ID_VaiTro INT PRIMARY KEY IDENTITY(1,1),
    TenVaiTro NVARCHAR(100)
);

CREATE TABLE TAIKHOAN (
    ID_TaiKhoan INT PRIMARY KEY IDENTITY(1,1),
    Email NVARCHAR(500) UNIQUE NOT NULL, -- Dùng làm tên đăng nhập
    MatKhau NVARCHAR(255) NOT NULL, -- Mật khẩu do admin quản lý
    ID_VaiTro INT NOT NULL,
    TrangThai BIT DEFAULT 1, -- 1: Hoạt động, 0: Bị vô hiệu hóa
    FOREIGN KEY (ID_VaiTro) REFERENCES VAITRO(ID_VaiTro)
);


CREATE TABLE NHANVIEN (
    ID_NhanVien INT PRIMARY KEY IDENTITY(1,1),
    HoTenNV NVARCHAR(500) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10) NOT NULL,
    CCCD NVARCHAR(20) UNIQUE NOT NULL,
    DienThoai NVARCHAR(15) UNIQUE NOT NULL,
    DiaChi NVARCHAR(500),
    Email NVARCHAR(500) UNIQUE NOT NULL,
    HinhAnh NVARCHAR(255),
    ID_VaiTro INT NOT NULL,
	TrangThai NVARCHAR(50) DEFAULT N'Đang làm việc',
    FOREIGN KEY (ID_VaiTro) REFERENCES VAITRO(ID_VaiTro)
);

CREATE TABLE BENHNHAN (
    ID_BenhNhan INT PRIMARY KEY IDENTITY(1,1),
    HoTenBN NVARCHAR(500) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(10) NOT NULL,
    CCCD NVARCHAR(20) UNIQUE,
    DienThoai NVARCHAR(15) UNIQUE NOT NULL,
	DiaChi NVARCHAR(500),
    Email NVARCHAR(100) UNIQUE
);

CREATE TABLE DANHSACHTIEPNHAN (
    ID_BenhNhan INT NOT NULL,
    ThoiGianTiepNhan DATETIME NOT NULL,
    ID_NhanVien INT NOT NULL,
    PRIMARY KEY (ID_BenhNhan, ThoiGianTiepNhan),
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
    ID_BenhNhan INT NOT NULL,
    NgayKham DATE NOT NULL,
    TrieuChung NVARCHAR(255),
    ID_LoaiBenh INT NOT NULL,
    TienKham DECIMAL(10,2),
    TongTienThuoc DECIMAL(10,2),
    FOREIGN KEY (ID_BenhNhan) REFERENCES BENHNHAN(ID_BenhNhan),
	FOREIGN KEY (ID_LoaiBenh) REFERENCES LOAIBENH(ID_LoaiBenh)
);

CREATE TABLE DANHSACHHENKHAM (
    ID_HenKham INT PRIMARY KEY IDENTITY(1,1),
    ID_BenhNhan INT NOT NULL,
    NgayHen DATETIME NOT NULL,
    TrangThai NVARCHAR(50) DEFAULT N'Hẹn khám thành công',
    ID_NhanVien INT NOT NULL,
    FOREIGN KEY (ID_BenhNhan) REFERENCES BENHNHAN(ID_BenhNhan),
    FOREIGN KEY (ID_NhanVien) REFERENCES NHANVIEN(ID_NhanVien)
);

CREATE TABLE HOADON (
    ID_HoaDon INT PRIMARY KEY IDENTITY(1,1),
    ID_PhieuKham INT NOT NULL,
    ID_BenhNhan INT NOT NULL,
    ID_NhanVien INT NOT NULL,
    NgayHoaDon DATE NOT NULL,
    TongTien DECIMAL(10,2),
    FOREIGN KEY (ID_PhieuKham) REFERENCES PHIEUKHAM(ID_PhieuKham),
    FOREIGN KEY (ID_BenhNhan) REFERENCES BENHNHAN(ID_BenhNhan),
    FOREIGN KEY (ID_NhanVien) REFERENCES NHANVIEN(ID_NhanVien)
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
    SoLuongTon INT NOT NULL,
    DonGiaNhap DECIMAL(10,2),
    HanSuDung DATE,
    HinhAnh NVARCHAR(255),
    TyLeGiaBan DECIMAL(10,2),
	DonGiaBan DECIMAL(10,2) NULL,
    FOREIGN KEY (ID_DVT) REFERENCES DVT(ID_DVT),
    FOREIGN KEY (ID_CachDung) REFERENCES CACHDUNG(ID_CachDung)
);
ALTER TABLE THUOC ADD IsDeleted BIT NOT NULL DEFAULT 0;


CREATE TABLE TOATHUOC (
    ID_PhieuKham INT NOT NULL,
    ID_Thuoc INT NOT NULL,
    SoLuong INT NOT NULL,
	DonGiaBan_LucMua DECIMAL(10,2),
    TienThuoc DECIMAL(10,2),
    PRIMARY KEY (ID_PhieuKham, ID_Thuoc),
    FOREIGN KEY (ID_PhieuKham) REFERENCES PHIEUKHAM(ID_PhieuKham),
    FOREIGN KEY (ID_Thuoc) REFERENCES THUOC(ID_Thuoc)
);

CREATE TABLE PHIEUNHAPTHUOC (
    ID_PhieuNhapThuoc INT PRIMARY KEY IDENTITY(1,1),
    NgayNhap DATE NOT NULL,
    TongTienNhap DECIMAL(10,2)
);
ALTER TABLE PHIEUNHAPTHUOC
ADD CONSTRAINT DF_TongTienNhap DEFAULT 0 FOR TongTienNhap;



CREATE TABLE CHITIETPHIEUNHAPTHUOC (
    ID_PhieuNhapThuoc INT NOT NULL,
    ID_Thuoc INT NOT NULL,
    SoLuongNhap INT NOT NULL,
    DonGiaNhap DECIMAL(10,2) NOT NULL,
	ThanhTien DECIMAL(10,2),
    PRIMARY KEY (ID_PhieuNhapThuoc, ID_Thuoc),
    FOREIGN KEY (ID_PhieuNhapThuoc) REFERENCES PHIEUNHAPTHUOC(ID_PhieuNhapThuoc),
    FOREIGN KEY (ID_Thuoc) REFERENCES THUOC(ID_Thuoc)
);

CREATE TABLE BAOCAODOANHTHU (
    ID_BCDT INT PRIMARY KEY IDENTITY(1,1),
    Ngay INT NOT NULL,
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    SoBenhNhan INT NOT NULL,
    DoanhThu DECIMAL(10,2) NOT NULL,
    TyLe DECIMAL(5,2) NOT NULL
);

CREATE TABLE BAOCAOSUDUNGTHUOC (
    ID_BCSDT INT PRIMARY KEY IDENTITY(1,1),
    Ngay INT NOT NULL,
    Thang INT NOT NULL,
    Nam INT NOT NULL,
    ID_Thuoc INT NOT NULL,
    TongSoLuong INT NOT NULL,
    SoLanDung INT NOT NULL,
    FOREIGN KEY (ID_Thuoc) REFERENCES THUOC(ID_Thuoc)
);
-----------------------------------------------------------------------


--------------------------QUI DINH, RANG BUOC--------------------------
CREATE TABLE QUI_DINH (
    ID_QD INT PRIMARY KEY IDENTITY(1,1),
    TenQuiDinh NVARCHAR(100) UNIQUE NOT NULL,
    GiaTri INT NOT NULL
);

INSERT INTO QUI_DINH (TenQuiDinh, GiaTri) VALUES 
('SoLuongLichHenToiDa', 10),
('SoLuongPhieuKhamToiDa', 40),
('SoLoaiBenh', 5),
('SoLoaiThuoc', 30),
('SoDonViTinh', 2),
('SoCachDung', 4),
('TienKham', 30000);

CREATE TRIGGER trg_CalculateDonGiaBan
ON THUOC
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE THUOC
    SET DonGiaBan = DonGiaNhap * TyLeGiaBan
    WHERE ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;

CREATE TRIGGER trg_CalculateTienThuoc
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


CREATE TRIGGER trg_UpdateTongTienThuoc
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

CREATE TRIGGER trg_CalculateThanhTien
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

CREATE TRIGGER trg_UpdateSoLuongTon
ON TOATHUOC
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE THUOC
    SET SoLuongTon = SoLuongTon - t.SoLuong
    FROM THUOC th
    JOIN TOATHUOC t ON th.ID_Thuoc = t.ID_Thuoc
    WHERE t.ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;

CREATE TRIGGER trg_UpdateSoLuongNhap
ON CHITIETPHIEUNHAPTHUOC
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE THUOC
    SET SoLuongTon = SoLuongTon + c.SoLuongNhap
    FROM THUOC th
    JOIN CHITIETPHIEUNHAPTHUOC c ON th.ID_Thuoc = c.ID_Thuoc
    WHERE c.ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;

CREATE TRIGGER trg_UpdateDonGiaNhap
ON CHITIETPHIEUNHAPTHUOC
AFTER INSERT, UPDATE
AS
BEGIN
    -- Cập nhật DonGiaNhap của THUOC với giá nhập mới nhất
    UPDATE THUOC
    SET DonGiaNhap = (
        SELECT TOP 1 c.DonGiaNhap
        FROM CHITIETPHIEUNHAPTHUOC c
        WHERE c.ID_Thuoc = THUOC.ID_Thuoc
        ORDER BY c.ID_PhieuNhapThuoc DESC
    )
    WHERE ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);

    -- Cập nhật lại DonGiaBan nhưng không thay đổi các giá trị cũ trong TOATHUOC
    UPDATE THUOC
    SET DonGiaBan = DonGiaNhap * TyLeGiaBan
    WHERE ID_Thuoc IN (SELECT ID_Thuoc FROM inserted);
END;



CREATE TRIGGER trg_SetTienKham
ON PHIEUKHAM
AFTER INSERT
AS
BEGIN
    DECLARE @TienKham DECIMAL(10,2);

    -- Lấy giá trị tiền khám từ bảng QUI_DINH
    SELECT @TienKham = GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'TienKham';

    -- Cập nhật tiền khám cho các phiếu khám mới
    UPDATE PHIEUKHAM
    SET TienKham = @TienKham
    WHERE ID_PhieuKham IN (SELECT ID_PhieuKham FROM inserted);
END;

CREATE TRIGGER trg_CalculateTongTienHoaDon
ON HOADON
AFTER INSERT, UPDATE
AS
BEGIN
    UPDATE HOADON
    SET TongTien = (
        SELECT pk.TienKham + COALESCE(pk.TongTienThuoc, 0)
        FROM PHIEUKHAM pk
        WHERE pk.ID_PhieuKham = HOADON.ID_PhieuKham
    )
    WHERE ID_PhieuKham IN (SELECT ID_PhieuKham FROM inserted);
END;



CREATE TRIGGER trg_CheckLichHen
ON DANHSACHHENKHAM
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @MaxLimit INT;

    -- Lấy giới hạn số lượng lịch hẹn từ bảng QUI_DINH
    SELECT @MaxLimit = GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'SoLuongLichHenToiDa';

    -- Nếu có ngày nào trong tập `inserted` đã đạt giới hạn, từ chối chèn dữ liệu
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN (
            SELECT NgayHen, COUNT(*) AS SoLuong 
            FROM DANHSACHHENKHAM 
            GROUP BY NgayHen
        ) p ON i.NgayHen = p.NgayHen
        WHERE p.SoLuong >= @MaxLimit
    )
    BEGIN
        RAISERROR(N'Số lượng lịch hẹn trong ngày đã đạt giới hạn!', 16, 1);
        RETURN;
    END

    -- Chèn dữ liệu hợp lệ vào bảng
    INSERT INTO DANHSACHHENKHAM (ID_BenhNhan, NgayHen, ID_NhanVien) -- Thêm các cột khác ngoài ID_HenKham
    SELECT ID_BenhNhan, NgayHen, ID_NhanVien FROM inserted;
END;


CREATE TRIGGER trg_CheckPhieuKham
ON PHIEUKHAM
INSTEAD OF INSERT
AS
BEGIN
    DECLARE @MaxLimit INT;

    -- Lấy giới hạn số lượng phiếu khám từ bảng QUI_DINH
    SELECT @MaxLimit = GiaTri FROM QUI_DINH WHERE TenQuiDinh = 'SoLuongPhieuKhamToiDa';

    -- Kiểm tra từng ngày trong tập dữ liệu chèn vào
    IF EXISTS (
        SELECT 1
        FROM inserted i
        JOIN (
            SELECT NgayKham, COUNT(*) AS SoLuong 
            FROM PHIEUKHAM 
            GROUP BY NgayKham
        ) p ON i.NgayKham = p.NgayKham
        WHERE p.SoLuong >= @MaxLimit
    )
    BEGIN
        RAISERROR(N'Số lượng phiếu khám trong ngày đã đạt giới hạn!', 16, 1);
        RETURN;
    END

	 -- Chèn dữ liệu hợp lệ vào bảng
    INSERT INTO PHIEUKHAM (ID_BenhNhan, NgayKham, TrieuChung, ID_LoaiBenh)  -- Thêm các cột khác ngoài ID_PhieuKham
    SELECT ID_BenhNhan, NgayKham, TrieuChung, ID_LoaiBenh FROM inserted;
END;



--NEU MUON THAY DOI QUI DINH: UPDATE QUI_DINH SET GiaTri = 50 WHERE TenQuiDinh = 'SoLuongPhieuKhamToiDa';

CREATE TRIGGER trg_AutoCreateAccount
ON NHANVIEN
AFTER INSERT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO TAIKHOAN (Email, MatKhau, ID_VaiTro, TrangThai)
    SELECT 
        i.Email, 
        '123456',  -- Mật khẩu mặc định là '123456' (Không mã hóa)
        i.ID_VaiTro, 
        1  -- Trạng thái tài khoản = 1 (Hoạt động)
    FROM inserted i
    WHERE i.Email IS NOT NULL;  -- Chỉ tạo tài khoản nếu nhân viên có email
END;

CREATE PROCEDURE sp_CapNhatBaoCaoSuDungThuoc
AS
BEGIN
    -- Xóa dữ liệu cũ trước khi cập nhật
    DELETE FROM BAOCAOSUDUNGTHUOC;

    -- Thêm dữ liệu mới từ PHIEUKHAM và thuốc sử dụng trong toa thuốc
    INSERT INTO BAOCAOSUDUNGTHUOC (Ngay, Thang, Nam, ID_Thuoc, TongSoLuong, SoLanDung)
    SELECT 
        DAY(pk.NgayKham) AS Ngay,   -- Lấy ngày (số nguyên)
        MONTH(pk.NgayKham) AS Thang, -- Lấy tháng
        YEAR(pk.NgayKham) AS Nam,    -- Lấy năm
        tt.ID_Thuoc,                 -- ID thuốc từ toa thuốc
        SUM(tt.SoLuong) AS TongSoLuong, -- Tổng số lượng thuốc đã sử dụng
        COUNT(*) AS SoLanDung           -- Số lần thuốc xuất hiện trong toa thuốc
    FROM PHIEUKHAM pk
    JOIN TOATHUOC tt ON pk.ID_PhieuKham = tt.ID_PhieuKham  -- Kết nối với đơn thuốc
    GROUP BY DAY(pk.NgayKham), MONTH(pk.NgayKham), YEAR(pk.NgayKham), tt.ID_Thuoc;
END;

CREATE PROCEDURE sp_CapNhatBaoCaoDoanhThu
AS
BEGIN
    -- Xóa dữ liệu cũ trước khi cập nhật
    DELETE FROM BAOCAODOANHTHU;

    -- Chèn dữ liệu mới vào bảng báo cáo doanh thu
    INSERT INTO BAOCAODOANHTHU (Ngay, Thang, Nam, SoBenhNhan, DoanhThu, TyLe)
    SELECT 
        DAY(hd.NgayHoaDon) AS Ngay,   -- Lấy ngày
        MONTH(hd.NgayHoaDon) AS Thang, -- Lấy tháng
        YEAR(hd.NgayHoaDon) AS Nam,    -- Lấy năm
        COUNT(hd.ID_HoaDon) AS SoBenhNhan, -- Số lượng hóa đơn (ứng với số bệnh nhân)
        SUM(hd.TongTien) AS DoanhThu,      -- Tổng doanh thu từ hóa đơn
        SUM(hd.TongTien) * 1.0 / 
        (SELECT SUM(TongTien) 
         FROM HOADON 
         WHERE MONTH(NgayHoaDon) = MONTH(hd.NgayHoaDon) 
         AND YEAR(NgayHoaDon) = YEAR(hd.NgayHoaDon)) AS TyLe  -- Tính tỷ lệ doanh thu
    FROM HOADON hd
    GROUP BY DAY(hd.NgayHoaDon), MONTH(hd.NgayHoaDon), YEAR(hd.NgayHoaDon);
END;


-----------------------------------------------------------------------


--------------------------INSERT DU LIEU--------------------------
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


INSERT INTO THUOC (TenThuoc, ID_DVT, ID_CachDung, ThanhPhan, XuatXu, SoLuongTon, DonGiaNhap, HanSuDung, HinhAnh, TyLeGiaBan)
VALUES 
(N'Paracetamol', 1, 1, N'Paracetamol 500mg', N'Việt Nam', 100, 1000, '2026-12-31', N'D:\THUOC\paracetamol_500mg.jpg', 1.5),
(N'Ibuprofen', 1, 1, N'Ibuprofen 200mg', N'Mỹ', 50, 2000, '2026-10-20', N'D:\THUOC\ibuprofen.jpg', 1.6),
(N'Amoxicillin', 1, 1, N'Amoxicillin 500mg', N'Pháp', 80, 1500, '2025-08-15', N'D:\THUOC\amoxicillin.jpg', 1.7),
(N'Cefuroxime', 1, 1, N'Cefuroxime 250mg', N'Anh', 40, 3000, '2026-07-10', N'D:\THUOC\cefuroxime.jpg', 1.8),
(N'Vitamin C', 1, 1, N'Acid ascorbic 500mg', N'Việt Nam', 120, 800, '2026-09-30', N'D:\THUOC\vitaminC.jpg', 1.4),
(N'ORS', 2, 2, N'Bù nước và điện giải', N'Việt Nam', 60, 5000, '2027-01-01', N'D:\THUOC\ors.jpg', 1.3),
(N'Oresol', 2, 2, N'Bù nước và điện giải', N'Mỹ', 30, 6000, '2027-02-15', N'D:\THUOC\oresol.jpg', 1.2),
(N'Azithromycin', 1, 1, N'Azithromycin 250mg', N'Ấn Độ', 70, 2500, '2026-06-25', N'D:\THUOC\azithromycin.jpg', 1.6),
(N'Clarithromycin', 1, 1, N'Clarithromycin 500mg', N'Nhật', 35, 4000, '2026-05-30', N'D:\THUOC\clarithromycin.jpg', 1.8),
(N'Ranitidine', 1, 1, N'Ranitidine 150mg', N'Việt Nam', 90, 1200, '2025-12-20', N'D:\THUOC\ranitidine.jpg', 1.5),
(N'Omeprazole', 1, 1, N'Omeprazole 20mg', N'Đức', 100, 1800, '2026-11-15', N'D:\THUOC\omeprazole.jpg', 1.6),
(N'Lansoprazole', 1, 1, N'Lansoprazole 30mg', N'Mỹ', 60, 2000, '2026-08-22', N'D:\THUOC\lansoprazol.jpg', 1.7),
(N'Losartan', 1, 1, N'Losartan 50mg', N'Anh', 80, 2500, '2026-09-10', N'D:\THUOC\losartan.jpg', 1.9),
(N'Amlodipine', 1, 1, N'Amlodipine 5mg', N'Nhật', 55, 2200, '2025-07-18', N'D:\THUOC\amlodipin.jpg', 1.8),
(N'Atenolol', 1, 1, N'Atenolol 50mg', N'Pháp', 40, 2100, '2025-11-25', N'D:\THUOC\Atenolol.jpg', 1.7),
(N'Glucose 5%', 2, 4, N'Dung dịch Glucose 5%', N'Việt Nam', 50, 7000, '2027-03-10', N'D:\THUOC\Glucose.jpg', 1.3),
(N'Natri clorid 0.9%', 2, 4, N'Dung dịch muối sinh lý', N'Việt Nam', 60, 5000, '2027-04-05', N'D:\THUOC\Natri_clorid.jpg', 1.2),
(N'Bromhexin', 1, 1, N'Bromhexin 8mg', N'Đức', 30, 1800, '2026-12-01', N'D:\THUOC\Bromhexin.jpg', 1.6),
(N'Dextromethorphan', 1, 1, N'Dextromethorphan 15mg', N'Việt Nam', 70, 1200, '2026-10-10', N'D:\THUOC\Dextromethorphan.jpg', 1.5),
(N'Codein', 1, 1, N'Codein 10mg', N'Mỹ', 50, 2500, '2025-09-25', N'D:\THUOC\Codein.jpg', 1.8),
(N'Levocetirizine', 1, 1, N'Levocetirizine 5mg', N'Anh', 80, 1400, '2026-05-15', N'D:\THUOC\Levocetirizine.png', 1.6),
(N'Loratadine', 1, 1, N'Loratadine 10mg', N'Nhật', 60, 1600, '2026-07-20', N'D:\THUOC\Loratadine.jpg', 1.7),
(N'Cetirizine', 1, 1, N'Cetirizine 10mg', N'Pháp', 50, 1700, '2026-08-30', N'D:\THUOC\Cetirizine.png', 1.8),
(N'Prednisolone', 1, 1, N'Prednisolone 5mg', N'Việt Nam', 40, 1300, '2025-06-10', N'D:\THUOC\Prednisolone.jpg', 1.5),
(N'Dexamethasone', 1, 1, N'Dexamethasone 0.5mg', N'Đức', 30, 1100, '2025-10-15', N'D:\THUOC\Dexamethasone.jpg', 1.4),
(N'Insulin', 2, 4, N'Insulin tiêm', N'Mỹ', 20, 50000, '2027-01-20', N'D:\THUOC\Insulin.png', 1.1),
(N'Adrenaline', 2, 4, N'Adrenaline tiêm', N'Anh', 25, 60000, '2026-12-10', N'D:\THUOC\Adrenaline.png', 1.1),
(N'Atropin', 2, 4, N'Atropin tiêm', N'Pháp', 30, 70000, '2026-10-25', N'D:\THUOC\Atropin.jpg', 1.1),
(N'Furosemide', 1, 1, N'Furosemide 40mg', N'Việt Nam', 50, 1900, '2025-09-05', N'D:\THUOC\Furosemide.jpg', 1.7),
(N'Aspirin', 1, 2, N'Acetylsalicylic Acid', N'Việt Nam', 100, 5000, '2026-12-31', N'D:\THUOC\Aspirin.jpg', 1.2);

INSERT INTO VAITRO (TenVaiTro) 
VALUES (N'Admin'), (N'Thu ngân'), (N'Nhân viên lễ tân');

INSERT INTO TAIKHOAN (Email, MatKhau, ID_VaiTro)
VALUES ('23520827@gm.uit.edu.vn','admin123', 1);

INSERT INTO NHANVIEN (HoTenNV, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email, HinhAnh, ID_VaiTro)
VALUES 
(N'Phạm Thị Hồng Hạnh', '2005-01-01', N'Nữ', '123456789101', '0987654321', N'123 Đường ABC, TP.HCM', '23520442@gm.uit.edu.vn', N'D:\IMAGE\nhanvien1.png', 2),
(N'Nguyễn Phương Nam', '2005-01-01', N'Nam', '123456789102', '0987654322', N'234 Đường BCD, TPHCM', '23520980@gm.uit.edu.vn', N'D:\IMAGE\nhanvien2.png', 2),
(N'Phạm Thị Kiều Diễm', '2005-06-09', N'Nữ', '123456789103', '0987654323', N'345 Đường CDE, TP.HCM', '23520286@gm.uit.edu.vn', N'D:\IMAGE\nhanvien3.png', 3),
(N'Nguyễn Trương Ngọc Hân', '2005-04-19', N'Nữ', '123456789104', '0987654324', N'456 Đường DEF, TP.HCM', '23520434@gm.uit.edu.vn', N'D:\IMAGE\nhanvien4.png', 3);

INSERT INTO BENHNHAN (HoTenBN, NgaySinh, GioiTinh, CCCD, DienThoai, DiaChi, Email)  
VALUES  
('Nguyen Van A', '1990-05-12', 'Nam', '123456789012', '0901234567', 'HCM', 'a@gmail.com'),
('Tran Thi B', '1985-07-20', N'Nữ', '223456789012', '0912345678', 'Hanoi', 'b@gmail.com'),
('Le Van C', '1992-03-25', 'Nam', '323456789012', '0923456789', 'Da Nang', 'c@gmail.com'),
('Pham Van D', '1988-01-14', 'Nam', '423456789012', '0934567890', 'Hai Phong', 'd@gmail.com'),
('Hoang Thi E', '1995-09-10', N'Nữ', '523456789012', '0945678901', 'Can Tho', 'e@gmail.com'),
('Bui Van F', '1993-07-07', 'Nam', '623456789012', '0956789012', 'Nha Trang', 'f@gmail.com'),
('Do Thi G', '1998-02-17', N'Nữ', '723456789012', '0967890123', 'Hue', 'g@gmail.com'),
('Nguyen Van H', '2000-06-05', 'Nam', '823456789012', '0978901234', 'Dong Nai', 'h@gmail.com'),
('Pham Thi I', '1987-11-23', N'Nữ', '923456789012', '0989012345', 'Vung Tau', 'i@gmail.com'),
('Tran Van J', '1991-04-12', 'Nam', '113456789012', '0990123456', 'Tay Ninh', 'j@gmail.com'),
('Hoang Van K', '1986-03-08', 'Nam', '213456789012', '0901111222', 'Quang Ninh', 'k@gmail.com'),
('Nguyen Thi L', '1994-08-18', N'Nữ', '313456789012', '0912222333', 'Lai Chau', 'l@gmail.com'),
('Pham Van M', '1999-05-29', 'Nam', '413456789012', '0923333444', 'Lang Son', 'm@gmail.com'),
('Do Thi N', '1997-07-14', N'Nữ', '513456789012', '0934444555', 'Ha Tinh', 'n@gmail.com'),
('Bui Van O', '1996-01-21', 'Nam', '613456789012', '0945555666', 'Nam Dinh', 'o@gmail.com'),
('Le Thi P', '1984-10-10', N'Nữ', '713456789012', '0956666777', 'Vinh Phuc', 'p@gmail.com'),
('Nguyen Van Q', '1983-12-30', 'Nam', '813456789012', '0967777888', 'Bac Lieu', 'q@gmail.com'),
('Tran Thi R', '2001-02-05', N'Nữ', '913456789012', '0978888999', 'Binh Phuoc', 'r@gmail.com'),
('Hoang Van S', '1990-09-09', 'Nam', '103456789012', '0989999000', 'Soc Trang', 's@gmail.com'),
('Pham Thi T', '2002-06-23', N'Nữ', '203456789012', '0990000111', 'Kien Giang', 't@gmail.com'),
('Nguyen Van U', '1995-04-04', 'Nam', '303456789012', '0901111223', 'Phu Yen', 'u@gmail.com'),
('Tran Thi V', '1989-07-15', N'Nữ', '403456789012', '0912222334', 'Gia Lai', 'v@gmail.com'),
('Hoang Van W', '1993-11-28', 'Nam', '503456789012', '0923333445', 'Dak Lak', 'w@gmail.com'),
('Pham Thi X', '1982-08-08', N'Nữ', '603456789012', '0934444556', 'Kon Tum', 'x@gmail.com'),
('Do Van Y', '2003-03-19', 'Nam', '703456789012', '0945555667', 'Nghe An', 'y@gmail.com'),
('Bui Thi Z', '1981-09-22', N'Nữ', '803456789012', '0956666778', 'Quang Tri', 'z@gmail.com'),
('Nguyen Van AA', '1998-12-07', 'Nam', '903456789012', '0967777889', 'Ben Tre', 'aa@gmail.com'),
('Tran Thi BB', '1990-01-16', N'Nữ', '113456789013', '0978888990', 'Quang Nam', 'bb@gmail.com'),
('Hoang Van CC', '1985-06-25', 'Nam', '213456789013', '0989999001', 'Tuyen Quang', 'cc@gmail.com'),
('Pham Thi DD', '1994-11-11', N'Nữ', '313456789013', '0990000112', 'Ca Mau', 'dd@gmail.com'),
('Nguyen Van EE', '1999-02-14', 'Nam', '413456789013', '0901111224', 'Binh Dinh', 'ee@gmail.com'),
('Tran Thi FF', '1988-05-20', N'Nữ', '513456789013', '0912222335', 'Thai Nguyen', 'ff@gmail.com'),
('Hoang Van GG', '1992-07-18', 'Nam', '613456789013', '0923333446', 'Bac Giang', 'gg@gmail.com'),
('Pham Thi HH', '1987-03-29', N'Nữ', '713456789013', '0934444557', 'An Giang', 'hh@gmail.com'),
('Le Van II', '2000-09-15', 'Nam', '813456789013', '0945555668', 'Lao Cai', 'ii@gmail.com'),
('Do Thi JJ', '1991-10-30', N'Nữ', '913456789013', '0956666779', 'Ha Giang', 'jj@gmail.com'),
('Bui Van KK', '1986-12-19', 'Nam', '103456789013', '0967777890', 'Hau Giang', 'kk@gmail.com'),
('Pham Thi LL', '1997-01-05', N'Nữ', '203456789013', '0978888901', 'Dong Thap', 'll@gmail.com'),
('Pham Thi Z', '2000-10-15', N'Nữ', '113456789112', '0999999999', 'Can Tho', 'zz@gmail.com'),
('Nguyễn Văn An', '1990-05-10', 'Nam', '122456789012', '0992345678', 'Hà Nội', 'an.nguyen@example.com'),
('Trần Thị Bích', '1985-08-22', N'Nữ', '987654321098', '0909654321', 'TP.HCM', 'bich.tran@example.com'),
('Lê Hoàng Dũng', '2000-02-15', 'Nam', '192837465012', '0909123456', 'Đà Nẵng', 'dung.le@example.com'),
('Phạm Thùy Linh', '1995-11-03', N'Nữ', '564738291045', '0911223344', 'Hải Phòng', 'linh.pham@example.com'),
('Võ Minh Trí', '1988-07-19', 'Nam', '748392615087', '0977885566', 'Cần Thơ', 'tri.vo@example.com'),
('Đặng Thị Hương', '1993-09-25', N'Nữ', '908172635049', '0966998877', 'Nghệ An', 'huong.dang@example.com'),
('Bùi Văn Cường', '1982-04-12', 'Nam', '627394105827', '0955667788', 'Quảng Ninh', 'cuong.bui@example.com'),
('Ngô Thanh Tuyền', '1998-06-30', N'Nữ', '382910574683', '0944556677', 'Bình Dương', 'tuyen.ngo@example.com'),
('Hoàng Văn Nam', '1979-12-05', 'Nam', '271093847562', '0933445566', 'Bắc Giang', 'nam.hoang@example.com'),
('Lý Thị Mai', '2001-03-18', N'Nữ', '472819503746', '0922334455', 'Huế', 'mai.ly@example.com');



INSERT INTO DANHSACHTIEPNHAN (ID_BenhNhan, ThoiGianTiepNhan, ID_NhanVien)
VALUES
(1, '2025-03-29 08:00:00', 3),
(2, '2025-03-29 08:15:00', 4),
(3, '2025-03-29 08:30:00', 3),
(4, '2025-03-29 08:45:00', 4),
(5, '2025-03-29 09:00:00', 3),
(6, '2025-03-29 09:15:00', 4),
(7, '2025-03-29 09:30:00', 3),
(8, '2025-03-29 09:45:00', 4),
(9, '2025-03-29 10:00:00', 3),
(10, '2025-03-29 10:15:00', 4),
(11, '2025-03-29 10:30:00', 3),
(12, '2025-03-29 10:45:00', 4),
(13, '2025-03-29 11:00:00', 3),
(14, '2025-03-29 11:15:00', 4),
(15, '2025-03-29 11:30:00', 3),
(16, '2025-03-29 11:45:00', 4),
(17, '2025-03-29 13:00:00', 3),
(18, '2025-03-29 13:15:00', 4),
(19, '2025-03-29 13:30:00', 3),
(20, '2025-03-29 13:45:00', 4),
(21, '2025-03-29 14:00:00', 3),
(22, '2025-03-29 14:15:00', 4),
(23, '2025-03-29 14:30:00', 3),
(24, '2025-03-29 14:45:00', 4),
(25, '2025-03-29 15:00:00', 3),
(26, '2025-03-29 15:15:00', 4),
(27, '2025-03-29 15:30:00', 3),
(28, '2025-03-29 15:45:00', 4),
(29, '2025-03-29 16:00:00', 3),
(30, '2025-03-29 16:15:00', 4),
(31, '2025-03-29 16:30:00', 3),
(32, '2025-03-29 16:45:00', 4),
(33, '2025-03-29 17:00:00', 3),
(34, '2025-03-29 17:15:00', 4),
(35, '2025-03-29 17:30:00', 3),
(36, '2025-03-29 17:45:00', 4),
(37, '2025-03-29 18:00:00', 3),
(38, '2025-03-29 18:15:00', 4),
(39, '2025-03-29 18:30:00', 3),
(40, '2025-03-29 18:45:00', 4),
(41, '2025-03-29 19:00:00', 3),
(42, '2025-03-29 19:15:00', 4),
(43, '2025-03-29 19:30:00', 3),
(44, '2025-03-29 19:45:00', 4),
(45, '2025-03-29 20:00:00', 3),
(46, '2025-03-29 20:15:00', 4),
(47, '2025-03-29 20:30:00', 3),
(48, '2025-03-29 20:45:00', 4),
(49, '2025-03-29 21:00:00', 3);

INSERT INTO PHIEUKHAM (ID_BenhNhan, NgayKham, TrieuChung, ID_LoaiBenh)
VALUES
(1, '2025-03-29 08:00:00', N'Sốt nhẹ, đau đầu', 1),
(2, '2025-03-29 08:15:00', N'Ho khan, mệt mỏi', 2),
(3, '2025-03-29 08:30:00', N'Đau họng, sốt', 3),
(4, '2025-03-29 08:45:00', N'Chóng mặt, buồn nôn', 1),
(5, '2025-03-29 09:00:00', N'Đau bụng, tiêu chảy', 2),
(6, '2025-03-29 09:15:00', N'Mệt mỏi, khó thở', 3),
(7, '2025-03-29 09:30:00', N'Đau ngực, tức ngực', 1),
(8, '2025-03-29 09:45:00', N'Hắt hơi, sổ mũi', 2),
(9, '2025-03-29 10:00:00', N'Ho ra máu, khó thở', 3),
(10, '2025-03-29 10:15:00', N'Đau khớp, sưng khớp', 1),
(11, '2025-03-29 10:30:00', N'Ngứa da, dị ứng', 2),
(12, '2025-03-29 10:45:00', N'Chóng mặt, hoa mắt', 3),
(13, '2025-03-29 11:00:00', N'Ho lâu ngày, khó thở', 1),
(14, '2025-03-29 11:15:00', N'Đau dạ dày, buồn nôn', 2),
(15, '2025-03-29 11:30:00', N'Rối loạn tiêu hóa', 3),
(16, '2025-03-29 11:45:00', N'Sốt cao, lạnh run', 1),
(17, '2025-03-29 13:00:00', N'Tê tay chân, mất ngủ', 2),
(18, '2025-03-29 13:15:00', N'Viêm họng, đau rát', 3),
(19, '2025-03-29 13:30:00', N'Huyết áp cao', 1),
(20, '2025-03-29 13:45:00', N'Đau lưng, nhức mỏi', 2),
(21, '2025-03-29 14:00:00', N'Đau đầu gối, cứng khớp', 3),
(22, '2025-03-29 14:15:00', N'Khó tiêu, đầy bụng', 1),
(23, '2025-03-29 14:30:00', N'Nổi mẩn đỏ, ngứa', 2),
(24, '2025-03-29 14:45:00', N'Mệt mỏi, thiếu máu', 3),
(25, '2025-03-29 15:00:00', N'Đau bụng, khó tiêu', 1),
(26, '2025-03-29 15:15:00', N'Mất ngủ kéo dài', 2),
(27, '2025-03-29 15:30:00', N'Hoa mắt, chóng mặt', 3),
(28, '2025-03-29 15:45:00', N'Nôn mửa, sốt nhẹ', 1),
(29, '2025-03-29 16:00:00', N'Tê bì chân tay', 2),
(30, '2025-03-29 16:15:00', N'Sưng đau khớp', 3),
(31, '2025-03-29 16:30:00', N'Khó thở, tức ngực', 1),
(32, '2025-03-29 16:45:00', N'Ngứa mắt, dị ứng', 2),
(33, '2025-03-29 17:00:00', N'Viêm xoang, nghẹt mũi', 3),
(34, '2025-03-29 17:15:00', N'Rối loạn tiêu hóa', 1),
(35, '2025-03-29 17:30:00', N'Mất ngủ, stress', 2),
(36, '2025-03-29 17:45:00', N'Huyết áp thấp', 3),
(37, '2025-03-29 18:00:00', N'Đau tim, khó thở', 1),
(38, '2025-03-29 18:15:00', N'Bỏng nhẹ, trầy xước', 2),
(39, '2025-03-29 18:30:00', N'Viêm phế quản', 3),
(40, '2025-03-29 18:45:00', N'Đau nửa đầu', 1),
(41, '2025-03-29 19:00:00', N'Khó thở, tức ngực', 2),
(42, '2025-03-29 19:15:00', N'Tiểu buốt, tiểu khó', 3),
(43, '2025-03-29 19:30:00', N'Viêm họng, sốt', 1),
(44, '2025-03-29 19:45:00', N'Đau lưng dưới', 2),
(45, '2025-03-29 20:00:00', N'Sưng chân, khó đi lại', 3),
(46, '2025-03-29 20:15:00', N'Rối loạn thần kinh', 1),
(47, '2025-03-29 20:30:00', N'Đau vai gáy', 2),
(48, '2025-03-29 20:45:00', N'Viêm da, bong tróc', 3),
(49, '2025-03-29 21:00:00', N'Bệnh dạ dày', 1);

INSERT INTO DANHSACHHENKHAM (ID_BenhNhan, NgayHen, ID_NhanVien)
VALUES
(41, '2025-03-30 08:00:00', 3),
(42, '2025-03-30 08:15:00', 4),
(43, '2025-03-30 08:30:00', 3),
(44, '2025-03-30 08:45:00', 4),
(45, '2025-03-30 09:00:00', 3),
(46, '2025-03-30 09:15:00', 4),
(47, '2025-03-30 09:30:00', 3),
(48, '2025-03-30 09:45:00', 4),
(49, '2025-03-30 10:00:00', 3);

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

INSERT INTO PHIEUNHAPTHUOC (NgayNhap)
VALUES 
('2025-03-30'),
('2025-03-31');

INSERT INTO CHITIETPHIEUNHAPTHUOC (ID_PhieuNhapThuoc, ID_Thuoc, SoLuongNhap, DonGiaNhap)
VALUES 
(1, 1, 50, 1000), -- Paracetamol
(1, 2, 30, 2000), -- Ibuprofen
(1, 3, 40, 1500), -- Amoxicillin
(1, 4, 20, 3000), -- Cefuroxime
(2, 5, 100, 800), -- Vitamin C
(2, 6, 10, 5000), -- ORS
(2, 7, 15, 6000), -- Oresol
(2, 8, 25, 2500); -- Azithromycin

INSERT INTO HOADON (ID_PhieuKham, ID_BenhNhan, ID_NhanVien, NgayHoaDon)
SELECT 
    ID_PhieuKham,
    ID_BenhNhan,
    CASE WHEN RAND() < 0.5 THEN 1 ELSE 2 END AS ID_NhanVien, 
    '2025-03-29'
FROM PHIEUKHAM
WHERE NgayKham = '2025-03-29';

UPDATE HOADON
SET ID_NhanVien = 2
WHERE ID_HoaDon >20

-----------------------------------------------------------------------



--------------------------TRUY VAN CAC BANG--------------------------

SELECT * FROM DVT
SELECT * FROM CACHDUNG
SELECT * FROM LOAIBENH
SELECT * FROM THUOC
SELECT TOP 1 * FROM THUOC WHERE TenThuoc = 'Paracetamol' AND IsDeleted = 0
SELECT TOP 1 t.HinhAnh FROM THUOC t 
WHERE t.TenThuoc = 'Paracetamol' AND t.IsDeleted = 0

SELECT * FROM NHANVIEN
SELECT * FROM VAITRO
SELECT * FROM TAIKHOAN
SELECT * FROM BENHNHAN
SELECT * FROM DANHSACHTIEPNHAN
SELECT * FROM PHIEUKHAM
SELECT * FROM DANHSACHHENKHAM
SELECT * FROM TOATHUOC
SELECT * FROM PHIEUNHAPTHUOC
SELECT * FROM CHITIETPHIEUNHAPTHUOC
SELECT * FROM HOADON
EXEC sp_CapNhatBaoCaoSuDungThuoc;
SELECT * FROM BAOCAOSUDUNGTHUOC
EXEC sp_CapNhatBaoCaoDoanhThu;
SELECT * FROM BAOCAODOANHTHU

-----------------------------------------------------------------------
