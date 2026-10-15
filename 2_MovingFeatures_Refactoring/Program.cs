using System;
using System.Collections.Generic;
using System.Text;

// Nhóm 2: MOVING FEATURES BETWEEN OBJECTS (refactoring.guru/refactoring/techniques/moving-features-between-objects)
// C# 7.3. Mỗi kỹ thuật có <TênKỹThuật>_Before và <TênKỹThuật>_After, đều có hàm Chay() trả về kết quả
// để Main so sánh: refactoring không được làm đổi hành vi.
namespace CuahangNongduoc.Refactoring.MovingFeatures
{
    public class ChiTiet
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public long DonGia { get; set; }
        public long ThanhTien { get { return (long)SoLuong * DonGia; } }
    }

    // =====================================================================
    // 1. MOVE METHOD - chuyển phương thức sang lớp mà nó dùng dữ liệu nhiều nhất
    // Vấn đề: HoaDon.TinhPhiGiaoHang chỉ dùng dữ liệu của KhachHang.
    // =====================================================================
    public static class MoveMethod_Before
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public double KhoangCachKm { get; set; }
        }

        public class HoaDon
        {
            public KhachHang Khach { get; set; }

            public long TinhPhiGiaoHang()
            {
                return Khach.KhoangCachKm > 20 ? 50000 : 20000;
            }
        }

        public static string Chay()
        {
            var hd = new HoaDon { Khach = new KhachHang { Ten = "Anh Bay", KhoangCachKm = 35 } };
            return "Phi giao hang: " + hd.TinhPhiGiaoHang();
        }
    }

    public static class MoveMethod_After
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public double KhoangCachKm { get; set; }

            // Phương thức đã chuyển về đây, nơi chứa dữ liệu nó cần
            public long TinhPhiGiaoHang()
            {
                return KhoangCachKm > 20 ? 50000 : 20000;
            }
        }

        public class HoaDon
        {
            public KhachHang Khach { get; set; }

            public long TinhPhiGiaoHang()
            {
                return Khach.TinhPhiGiaoHang();
            }
        }

        public static string Chay()
        {
            var hd = new HoaDon { Khach = new KhachHang { Ten = "Anh Bay", KhoangCachKm = 35 } };
            return "Phi giao hang: " + hd.TinhPhiGiaoHang();
        }
    }

    // =====================================================================
    // 2. MOVE FIELD - chuyển trường sang lớp mà nó thực sự thuộc về
    // Vấn đề: địa chỉ nhà cung cấp lại nằm trong SanPham.
    // =====================================================================
    public static class MoveField_Before
    {
        public class NhaCungCap
        {
            public string Ten { get; set; }
        }

        public class SanPham
        {
            public string Ten { get; set; }
            public NhaCungCap NhaCungCap { get; set; }
            public string DiaChiNhaCungCap { get; set; } // sai chỗ
        }

        public static string Chay()
        {
            var sp = new SanPham
            {
                Ten = "Regent 800WG",
                NhaCungCap = new NhaCungCap { Ten = "Cong ty A" },
                DiaChiNhaCungCap = "Long Xuyen"
            };
            return sp.Ten + " | " + sp.NhaCungCap.Ten + " | " + sp.DiaChiNhaCungCap;
        }
    }

    public static class MoveField_After
    {
        public class NhaCungCap
        {
            public string Ten { get; set; }
            public string DiaChi { get; set; } // đã chuyển về đúng lớp
        }

        public class SanPham
        {
            public string Ten { get; set; }
            public NhaCungCap NhaCungCap { get; set; }
        }

        public static string Chay()
        {
            var sp = new SanPham
            {
                Ten = "Regent 800WG",
                NhaCungCap = new NhaCungCap { Ten = "Cong ty A", DiaChi = "Long Xuyen" }
            };
            return sp.Ten + " | " + sp.NhaCungCap.Ten + " | " + sp.NhaCungCap.DiaChi;
        }
    }

    // =====================================================================
    // 3. EXTRACT CLASS - tách phần dữ liệu và hành vi riêng ra lớp mới
    // Vấn đề: KhachHang ôm cả các trường địa chỉ.
    // =====================================================================
    public static class ExtractClass_Before
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public string Duong { get; set; }
            public string Huyen { get; set; }
            public string Tinh { get; set; }

            public string DiaChiDayDu()
            {
                return Duong + ", " + Huyen + ", " + Tinh;
            }
        }

        public static string Chay()
        {
            var kh = new KhachHang { Ten = "Chi Tam", Duong = "123 Tran Hung Dao", Huyen = "Long Xuyen", Tinh = "An Giang" };
            return kh.Ten + ": " + kh.DiaChiDayDu();
        }
    }

    public static class ExtractClass_After
    {
        public class DiaChi
        {
            public string Duong { get; set; }
            public string Huyen { get; set; }
            public string Tinh { get; set; }

            public string DayDu()
            {
                return Duong + ", " + Huyen + ", " + Tinh;
            }
        }

        public class KhachHang
        {
            public string Ten { get; set; }
            public DiaChi DiaChi { get; set; }
        }

        public static string Chay()
        {
            var kh = new KhachHang
            {
                Ten = "Chi Tam",
                DiaChi = new DiaChi { Duong = "123 Tran Hung Dao", Huyen = "Long Xuyen", Tinh = "An Giang" }
            };
            return kh.Ten + ": " + kh.DiaChi.DayDu();
        }
    }

    // =====================================================================
    // 4. INLINE CLASS - gộp lớp quá nhỏ, không làm gì đáng kể, vào lớp dùng nó
    // (ngược lại với Extract Class)
    // =====================================================================
    public static class InlineClass_Before
    {
        public class SoDienThoai
        {
            public string So { get; set; }
        }

        public class KhachHang
        {
            public string Ten { get; set; }
            public SoDienThoai Sdt { get; set; }
        }

        public static string Chay()
        {
            var kh = new KhachHang { Ten = "Anh Bay", Sdt = new SoDienThoai { So = "0918070557" } };
            return kh.Ten + " - " + kh.Sdt.So;
        }
    }

    public static class InlineClass_After
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public string SoDienThoai { get; set; }
        }

        public static string Chay()
        {
            var kh = new KhachHang { Ten = "Anh Bay", SoDienThoai = "0918070557" };
            return kh.Ten + " - " + kh.SoDienThoai;
        }
    }

    // =====================================================================
    // 5. HIDE DELEGATE - che giấu chuỗi gọi a.b.c bằng một phương thức ủy quyền
    // Vấn đề: nơi gọi phải biết cấu trúc HoaDon -> KhachHang -> NhanVien.
    // =====================================================================
    public static class HideDelegate_Before
    {
        public class NhanVien { public string Ten { get; set; } }
        public class KhachHang { public NhanVien PhuTrach { get; set; } }
        public class HoaDon { public KhachHang Khach { get; set; } }

        public static string Chay()
        {
            var hd = new HoaDon { Khach = new KhachHang { PhuTrach = new NhanVien { Ten = "Lan" } } };
            return "Nhan vien phu trach: " + hd.Khach.PhuTrach.Ten; // client biết hết chuỗi
        }
    }

    public static class HideDelegate_After
    {
        public class NhanVien { public string Ten { get; set; } }

        public class KhachHang
        {
            public NhanVien PhuTrach { get; set; }
            public string TenNhanVienPhuTrach() { return PhuTrach.Ten; }
        }

        public class HoaDon
        {
            public KhachHang Khach { get; set; }
            public string TenNhanVienPhuTrach() { return Khach.TenNhanVienPhuTrach(); }
        }

        public static string Chay()
        {
            var hd = new HoaDon { Khach = new KhachHang { PhuTrach = new NhanVien { Ten = "Lan" } } };
            return "Nhan vien phu trach: " + hd.TenNhanVienPhuTrach(); // client chỉ làm việc với HoaDon
        }
    }

    // =====================================================================
    // 6. REMOVE MIDDLE MAN - bỏ lớp trung gian chỉ chuyển tiếp lời gọi
    // (ngược lại với Hide Delegate, dùng khi có quá nhiều phương thức ủy quyền)
    // =====================================================================
    public static class RemoveMiddleMan_Before
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public string DiaChi { get; set; }
            public string Sdt { get; set; }
        }

        public class HoaDon
        {
            private readonly KhachHang _khach;
            public HoaDon(KhachHang khach) { _khach = khach; }
            public string TenKhach() { return _khach.Ten; }
            public string DiaChiKhach() { return _khach.DiaChi; }
            public string SdtKhach() { return _khach.Sdt; }
        }

        public static string Chay()
        {
            var hd = new HoaDon(new KhachHang { Ten = "Anh Bay", DiaChi = "Long Xuyen", Sdt = "0918070557" });
            return hd.TenKhach() + " | " + hd.DiaChiKhach() + " | " + hd.SdtKhach();
        }
    }

    public static class RemoveMiddleMan_After
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public string DiaChi { get; set; }
            public string Sdt { get; set; }
        }

        public class HoaDon
        {
            public KhachHang Khach { get; private set; }
            public HoaDon(KhachHang khach) { Khach = khach; }
        }

        public static string Chay()
        {
            var hd = new HoaDon(new KhachHang { Ten = "Anh Bay", DiaChi = "Long Xuyen", Sdt = "0918070557" });
            return hd.Khach.Ten + " | " + hd.Khach.DiaChi + " | " + hd.Khach.Sdt;
        }
    }

    // =====================================================================
    // 7. INTRODUCE FOREIGN METHOD - thêm phương thức vào lớp client khi không
    //    sửa được lớp thư viện (ở đây là DateTime của .NET)
    // Vấn đề: đoạn tính "ngày cuối tháng" bị lặp lại ở nhiều nơi.
    // =====================================================================
    public static class IntroduceForeignMethod_Before
    {
        public static string Chay()
        {
            DateTime ngayBan = new DateTime(2026, 10, 15);
            DateTime hanThanhToan = new DateTime(ngayBan.Year, ngayBan.Month, DateTime.DaysInMonth(ngayBan.Year, ngayBan.Month));
            DateTime ngayChotSo = new DateTime(ngayBan.Year, ngayBan.Month, DateTime.DaysInMonth(ngayBan.Year, ngayBan.Month));
            return hanThanhToan.ToString("dd/MM/yyyy") + " | " + ngayChotSo.ToString("dd/MM/yyyy");
        }
    }

    public static class IntroduceForeignMethod_After
    {
        // Phương thức "ngoại lai": lẽ ra thuộc DateTime nhưng ta không sửa được DateTime
        private static DateTime NgayCuoiThang(DateTime ngay)
        {
            return new DateTime(ngay.Year, ngay.Month, DateTime.DaysInMonth(ngay.Year, ngay.Month));
        }

        public static string Chay()
        {
            DateTime ngayBan = new DateTime(2026, 10, 15);
            DateTime hanThanhToan = NgayCuoiThang(ngayBan);
            DateTime ngayChotSo = NgayCuoiThang(ngayBan);
            return hanThanhToan.ToString("dd/MM/yyyy") + " | " + ngayChotSo.ToString("dd/MM/yyyy");
        }
    }

    // =====================================================================
    // 8. INTRODUCE LOCAL EXTENSION - tạo lớp con/lớp bọc để gom nhiều phương thức
    //    bổ sung cho lớp ta không sửa được (ở đây là List<ChiTiet>)
    // Vấn đề: các hàm tiện ích về danh sách chi tiết nằm rải rác ở lớp tĩnh.
    // =====================================================================
    public static class IntroduceLocalExtension_Before
    {
        public static class TienIch
        {
            public static long TongTien(List<ChiTiet> ds)
            {
                long tong = 0;
                foreach (var ct in ds) tong += ct.ThanhTien;
                return tong;
            }

            public static string MatHangGiaTriNhat(List<ChiTiet> ds)
            {
                ChiTiet max = null;
                foreach (var ct in ds)
                    if (max == null || ct.ThanhTien > max.ThanhTien) max = ct;
                return max == null ? "" : max.Ten;
            }
        }

        public static string Chay()
        {
            var ds = new List<ChiTiet>
            {
                new ChiTiet { Ten = "Regent 800WG", SoLuong = 20, DonGia = 85000 },
                new ChiTiet { Ten = "Phan NPK", SoLuong = 10, DonGia = 320000 }
            };
            return TienIch.TongTien(ds) + " | " + TienIch.MatHangGiaTriNhat(ds);
        }
    }

    public static class IntroduceLocalExtension_After
    {
        // Lớp mở rộng cục bộ: kế thừa List<ChiTiet> rồi thêm các phương thức riêng
        public class DanhSachChiTiet : List<ChiTiet>
        {
            public long TongTien()
            {
                long tong = 0;
                foreach (var ct in this) tong += ct.ThanhTien;
                return tong;
            }

            public string MatHangGiaTriNhat()
            {
                ChiTiet max = null;
                foreach (var ct in this)
                    if (max == null || ct.ThanhTien > max.ThanhTien) max = ct;
                return max == null ? "" : max.Ten;
            }
        }

        public static string Chay()
        {
            var ds = new DanhSachChiTiet
            {
                new ChiTiet { Ten = "Regent 800WG", SoLuong = 20, DonGia = 85000 },
                new ChiTiet { Ten = "Phan NPK", SoLuong = 10, DonGia = 320000 }
            };
            return ds.TongTien() + " | " + ds.MatHangGiaTriNhat();
        }
    }

    // =====================================================================
    // CHƯƠNG TRÌNH CHẠY THỬ
    // =====================================================================
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Kiem("1. Move Method", MoveMethod_Before.Chay(), MoveMethod_After.Chay());
            Kiem("2. Move Field", MoveField_Before.Chay(), MoveField_After.Chay());
            Kiem("3. Extract Class", ExtractClass_Before.Chay(), ExtractClass_After.Chay());
            Kiem("4. Inline Class", InlineClass_Before.Chay(), InlineClass_After.Chay());
            Kiem("5. Hide Delegate", HideDelegate_Before.Chay(), HideDelegate_After.Chay());
            Kiem("6. Remove Middle Man", RemoveMiddleMan_Before.Chay(), RemoveMiddleMan_After.Chay());
            Kiem("7. Introduce Foreign Method", IntroduceForeignMethod_Before.Chay(), IntroduceForeignMethod_After.Chay());
            Kiem("8. Introduce Local Extension", IntroduceLocalExtension_Before.Chay(), IntroduceLocalExtension_After.Chay());

            Console.ReadKey();
        }

        // Kết quả cũ và mới giống nhau => refactoring không làm đổi hành vi.
        static void Kiem(string tenKyThuat, string truoc, string sau)
        {
            Console.WriteLine("[" + (truoc == sau ? "OK  " : "LOI ") + "] " + tenKyThuat + "  ->  " + sau);
        }
    }
}
