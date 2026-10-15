using System;
using System.Collections.Generic;
using System.Text;

// Nhóm 1: COMPOSING METHODS (refactoring.guru/refactoring/techniques/composing-methods)
// Viết theo C# 7.3 để chạy được trên project Console App (.NET Framework) mặc định.
// Mỗi kỹ thuật có 2 lớp: <TênKỹThuật>_Before (code cũ) và <TênKỹThuật>_After (code sau khi refactor).
// Hàm Main chạy cả hai và so sánh kết quả: refactoring chỉ đổi cấu trúc, KHÔNG được đổi hành vi.
namespace CuahangNongduoc.Refactoring.ComposingMethods
{
    public class ChiTiet
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public long DonGia { get; set; }
        public long ThanhTien { get { return (long)SoLuong * DonGia; } }
    }

    // =====================================================================
    // 1. EXTRACT METHOD - tách một đoạn code thành phương thức riêng có tên rõ nghĩa
    // Vấn đề: hàm in hóa đơn làm 3 việc (tiêu đề, chi tiết, tổng) trong 1 khối dài.
    // =====================================================================
    public static class ExtractMethod_Before
    {
        public static string InHoaDon(string khach, List<ChiTiet> ds)
        {
            var sb = new StringBuilder();
            // in tieu de
            sb.AppendLine("HOA DON BAN HANG");
            sb.AppendLine("Khach hang: " + khach);
            // in chi tiet
            long tong = 0;
            foreach (var ct in ds)
            {
                sb.AppendLine(ct.Ten + " x" + ct.SoLuong + " = " + ct.ThanhTien);
                tong += ct.ThanhTien;
            }
            // in tong
            sb.AppendLine("Tong cong: " + tong);
            return sb.ToString();
        }
    }

    public static class ExtractMethod_After
    {
        public static string InHoaDon(string khach, List<ChiTiet> ds)
        {
            var sb = new StringBuilder();
            InTieuDe(sb, khach);
            long tong = InChiTiet(sb, ds);
            InTong(sb, tong);
            return sb.ToString();
        }

        static void InTieuDe(StringBuilder sb, string khach)
        {
            sb.AppendLine("HOA DON BAN HANG");
            sb.AppendLine("Khach hang: " + khach);
        }

        static long InChiTiet(StringBuilder sb, List<ChiTiet> ds)
        {
            long tong = 0;
            foreach (var ct in ds)
            {
                sb.AppendLine(ct.Ten + " x" + ct.SoLuong + " = " + ct.ThanhTien);
                tong += ct.ThanhTien;
            }
            return tong;
        }

        static void InTong(StringBuilder sb, long tong)
        {
            sb.AppendLine("Tong cong: " + tong);
        }
    }

    // =====================================================================
    // 2. INLINE METHOD - bỏ phương thức quá đơn giản, đưa thân hàm vào nơi gọi
    // Vấn đề: NhieuHonNamLanGiaoTre chỉ là 1 phép so sánh, không đáng có hàm riêng.
    // =====================================================================
    public static class InlineMethod_Before
    {
        public static int XepHangNhaCungCap(int soLanGiaoTre)
        {
            return NhieuHonNamLanGiaoTre(soLanGiaoTre) ? 2 : 1;
        }

        static bool NhieuHonNamLanGiaoTre(int soLanGiaoTre)
        {
            return soLanGiaoTre > 5;
        }
    }

    public static class InlineMethod_After
    {
        public static int XepHangNhaCungCap(int soLanGiaoTre)
        {
            return soLanGiaoTre > 5 ? 2 : 1;
        }
    }

    // =====================================================================
    // 3. EXTRACT VARIABLE - đặt tên cho biểu thức phức tạp bằng biến cục bộ
    // Vấn đề: điều kiện giảm giá dài, khó đọc.
    // =====================================================================
    public static class ExtractVariable_Before
    {
        public static double TyLeGiam(string tenSanPham, int soLuong, DateTime ngayBan)
        {
            if (tenSanPham.ToUpper().IndexOf("THUOC") >= 0 && soLuong > 10 && ngayBan.DayOfWeek != DayOfWeek.Sunday)
                return 0.05;
            return 0;
        }
    }

    public static class ExtractVariable_After
    {
        public static double TyLeGiam(string tenSanPham, int soLuong, DateTime ngayBan)
        {
            bool laThuoc = tenSanPham.ToUpper().IndexOf("THUOC") >= 0;
            bool muaSi = soLuong > 10;
            bool ngayLamViec = ngayBan.DayOfWeek != DayOfWeek.Sunday;

            if (laThuoc && muaSi && ngayLamViec)
                return 0.05;
            return 0;
        }
    }

    // =====================================================================
    // 4. INLINE TEMP - bỏ biến tạm chỉ gán 1 lần rồi dùng ngay
    // =====================================================================
    public static class InlineTemp_Before
    {
        public static bool LaHangGiaTri(ChiTiet ct)
        {
            long gia = ct.DonGia;
            return gia > 100000;
        }
    }

    public static class InlineTemp_After
    {
        public static bool LaHangGiaTri(ChiTiet ct)
        {
            return ct.DonGia > 100000;
        }
    }

    // =====================================================================
    // 5. REPLACE TEMP WITH QUERY - thay biến tạm bằng phương thức truy vấn
    // Vấn đề: biến tong chỉ tồn tại trong hàm, không dùng lại được ở hàm khác.
    // =====================================================================
    public static class ReplaceTempWithQuery_Before
    {
        public static double TienSauGiam(ChiTiet ct)
        {
            long tong = (long)ct.SoLuong * ct.DonGia;
            if (tong > 1000000)
                return tong * 0.95;
            return tong * 0.98;
        }
    }

    public static class ReplaceTempWithQuery_After
    {
        public static double TienSauGiam(ChiTiet ct)
        {
            if (TongTien(ct) > 1000000)
                return TongTien(ct) * 0.95;
            return TongTien(ct) * 0.98;
        }

        static long TongTien(ChiTiet ct)
        {
            return (long)ct.SoLuong * ct.DonGia;
        }
    }

    // =====================================================================
    // 6. SPLIT TEMPORARY VARIABLE - mỗi biến chỉ giữ 1 ý nghĩa
    // Vấn đề: biến temp bị dùng lại cho 2 mục đích (thành tiền rồi tiền VAT).
    // =====================================================================
    public static class SplitTemporaryVariable_Before
    {
        public static string TinhTien(int soLuong, long donGia)
        {
            long temp = (long)soLuong * donGia;
            string kq = "Thanh tien: " + temp;
            temp = temp * 8 / 100;
            kq += " | VAT 8%: " + temp;
            return kq;
        }
    }

    public static class SplitTemporaryVariable_After
    {
        public static string TinhTien(int soLuong, long donGia)
        {
            long thanhTien = (long)soLuong * donGia;
            string kq = "Thanh tien: " + thanhTien;
            long tienVat = thanhTien * 8 / 100;
            kq += " | VAT 8%: " + tienVat;
            return kq;
        }
    }

    // =====================================================================
    // 7. REMOVE ASSIGNMENTS TO PARAMETERS - không gán lại giá trị cho tham số
    // =====================================================================
    public static class RemoveAssignmentsToParameters_Before
    {
        public static long GiaBan(long giaBan, int soLuong)
        {
            if (soLuong > 10)
                giaBan -= 5000;
            return giaBan;
        }
    }

    public static class RemoveAssignmentsToParameters_After
    {
        public static long GiaBan(long giaBan, int soLuong)
        {
            long giaSauGiam = giaBan;
            if (soLuong > 10)
                giaSauGiam -= 5000;
            return giaSauGiam;
        }
    }

    // =====================================================================
    // 8. REPLACE METHOD WITH METHOD OBJECT - chuyển hàm dài nhiều biến cục bộ
    //    thành một lớp riêng, biến cục bộ thành trường, rồi tách nhỏ hàm
    // =====================================================================
    public static class ReplaceMethodWithMethodObject_Before
    {
        public static double TinhGiaBan(double giaNhap, double tyLeLoi, double phiVanChuyen, double tyLeThue)
        {
            double loiNhuan = giaNhap * tyLeLoi;
            double giaTruocThue = giaNhap + loiNhuan + phiVanChuyen;
            double thue = giaTruocThue * tyLeThue;
            double giaBan = giaTruocThue + thue;
            if (giaBan > 1000000)
                giaBan = Math.Round(giaBan / 1000) * 1000;
            return giaBan;
        }
    }

    public class TinhGiaBanObject
    {
        private readonly double _giaNhap;
        private readonly double _tyLeLoi;
        private readonly double _phiVanChuyen;
        private readonly double _tyLeThue;

        public TinhGiaBanObject(double giaNhap, double tyLeLoi, double phiVanChuyen, double tyLeThue)
        {
            _giaNhap = giaNhap;
            _tyLeLoi = tyLeLoi;
            _phiVanChuyen = phiVanChuyen;
            _tyLeThue = tyLeThue;
        }

        public double Tinh()
        {
            double giaTruocThue = TinhGiaTruocThue();
            double giaBan = giaTruocThue + TinhThue(giaTruocThue);
            return LamTronGiaLon(giaBan);
        }

        private double TinhLoiNhuan() { return _giaNhap * _tyLeLoi; }

        private double TinhGiaTruocThue() { return _giaNhap + TinhLoiNhuan() + _phiVanChuyen; }

        private double TinhThue(double giaTruocThue) { return giaTruocThue * _tyLeThue; }

        private static double LamTronGiaLon(double giaBan)
        {
            return giaBan > 1000000 ? Math.Round(giaBan / 1000) * 1000 : giaBan;
        }
    }

    public static class ReplaceMethodWithMethodObject_After
    {
        public static double TinhGiaBan(double giaNhap, double tyLeLoi, double phiVanChuyen, double tyLeThue)
        {
            return new TinhGiaBanObject(giaNhap, tyLeLoi, phiVanChuyen, tyLeThue).Tinh();
        }
    }

    // =====================================================================
    // 9. SUBSTITUTE ALGORITHM - thay thuật toán rối bằng cách đơn giản hơn
    // Vấn đề: chuỗi if lặp lại để tra loại thuốc, thêm loại mới phải sửa hàm.
    // =====================================================================
    public static class SubstituteAlgorithm_Before
    {
        public static string TimLoaiThuoc(string ten)
        {
            if (ten == "Regent 800WG") return "Thuoc tru sau";
            if (ten == "Glyphosate") return "Thuoc diet co";
            if (ten == "Validamycin") return "Thuoc tru benh";
            return "Khac";
        }
    }

    public static class SubstituteAlgorithm_After
    {
        private static readonly Dictionary<string, string> LoaiThuoc = new Dictionary<string, string>
        {
            { "Regent 800WG", "Thuoc tru sau" },
            { "Glyphosate", "Thuoc diet co" },
            { "Validamycin", "Thuoc tru benh" }
        };

        public static string TimLoaiThuoc(string ten)
        {
            string loai;
            return LoaiThuoc.TryGetValue(ten, out loai) ? loai : "Khac";
        }
    }

    // =====================================================================
    // CHƯƠNG TRÌNH CHẠY THỬ: so sánh kết quả code cũ và code mới
    // =====================================================================
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            var ds = new List<ChiTiet>
            {
                new ChiTiet { Ten = "Regent 800WG", SoLuong = 20, DonGia = 85000 },
                new ChiTiet { Ten = "Phan NPK", SoLuong = 10, DonGia = 320000 }
            };
            var hang = new ChiTiet { Ten = "Glyphosate", SoLuong = 12, DonGia = 65000 };
            var ngayThuHai = new DateTime(2026, 10, 12); // thứ Hai
            var ngayChuNhat = new DateTime(2026, 10, 11); // Chủ nhật

            Kiem("1. Extract Method",
                ExtractMethod_Before.InHoaDon("Anh Bay", ds),
                ExtractMethod_After.InHoaDon("Anh Bay", ds));

            Kiem("2. Inline Method",
                InlineMethod_Before.XepHangNhaCungCap(7) + "," + InlineMethod_Before.XepHangNhaCungCap(3),
                InlineMethod_After.XepHangNhaCungCap(7) + "," + InlineMethod_After.XepHangNhaCungCap(3));

            Kiem("3. Extract Variable",
                ExtractVariable_Before.TyLeGiam("Thuoc tru sau", 20, ngayThuHai) + "," + ExtractVariable_Before.TyLeGiam("Thuoc tru sau", 20, ngayChuNhat),
                ExtractVariable_After.TyLeGiam("Thuoc tru sau", 20, ngayThuHai) + "," + ExtractVariable_After.TyLeGiam("Thuoc tru sau", 20, ngayChuNhat));

            Kiem("4. Inline Temp",
                InlineTemp_Before.LaHangGiaTri(ds[0]) + "," + InlineTemp_Before.LaHangGiaTri(hang),
                InlineTemp_After.LaHangGiaTri(ds[0]) + "," + InlineTemp_After.LaHangGiaTri(hang));

            Kiem("5. Replace Temp with Query",
                ReplaceTempWithQuery_Before.TienSauGiam(ds[1]) + "," + ReplaceTempWithQuery_Before.TienSauGiam(hang),
                ReplaceTempWithQuery_After.TienSauGiam(ds[1]) + "," + ReplaceTempWithQuery_After.TienSauGiam(hang));

            Kiem("6. Split Temporary Variable",
                SplitTemporaryVariable_Before.TinhTien(20, 85000),
                SplitTemporaryVariable_After.TinhTien(20, 85000));

            Kiem("7. Remove Assignments to Parameters",
                RemoveAssignmentsToParameters_Before.GiaBan(85000, 20) + "," + RemoveAssignmentsToParameters_Before.GiaBan(85000, 5),
                RemoveAssignmentsToParameters_After.GiaBan(85000, 20) + "," + RemoveAssignmentsToParameters_After.GiaBan(85000, 5));

            Kiem("8. Replace Method with Method Object",
                ReplaceMethodWithMethodObject_Before.TinhGiaBan(900000, 0.1, 30000, 0.08),
                ReplaceMethodWithMethodObject_After.TinhGiaBan(900000, 0.1, 30000, 0.08));

            Kiem("9. Substitute Algorithm",
                SubstituteAlgorithm_Before.TimLoaiThuoc("Glyphosate") + "," + SubstituteAlgorithm_Before.TimLoaiThuoc("Khong ro"),
                SubstituteAlgorithm_After.TimLoaiThuoc("Glyphosate") + "," + SubstituteAlgorithm_After.TimLoaiThuoc("Khong ro"));

            Console.ReadKey();
        }

        // Nếu kết quả cũ và mới giống nhau => refactoring an toàn (không đổi hành vi).
        static void Kiem(string tenKyThuat, object truoc, object sau)
        {
            bool giong = Equals(truoc, sau);
            Console.WriteLine("[" + (giong ? "OK  " : "LOI ") + "] " + tenKyThuat);
            Console.WriteLine("       Ket qua: " + sau.ToString().TrimEnd());
        }
    }
}
