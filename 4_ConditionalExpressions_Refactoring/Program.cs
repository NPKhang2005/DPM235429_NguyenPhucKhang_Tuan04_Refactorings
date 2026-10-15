using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

// Nhóm 4: SIMPLIFYING CONDITIONAL EXPRESSIONS (refactoring.guru/refactoring/techniques/simplifying-conditional-expressions) - 8 kỹ thuật
// C# 7.3. Mỗi kỹ thuật có <TênKỹThuật>_Before và <TênKỹThuật>_After, đều có hàm Chay().
namespace CuahangNongduoc.Refactoring.ConditionalExpressions
{
    // =====================================================================
    // 1. CONSOLIDATE CONDITIONAL EXPRESSION - gộp các điều kiện cùng trả một kết quả
    // =====================================================================
    public static class ConsolidateConditional_Before
    {
        public static long PhiGiaoHang(double km, long tongTien, bool laKhachVip)
        {
            if (tongTien > 2000000) return 0;
            if (laKhachVip) return 0;
            if (km < 2) return 0;
            return 30000;
        }

        public static string Chay()
        {
            return PhiGiaoHang(10, 500000, false) + "," + PhiGiaoHang(10, 3000000, false) + "," + PhiGiaoHang(1, 500000, false);
        }
    }

    public static class ConsolidateConditional_After
    {
        private static bool DuocMienPhi(double km, long tongTien, bool laKhachVip)
        {
            return tongTien > 2000000 || laKhachVip || km < 2;
        }

        public static long PhiGiaoHang(double km, long tongTien, bool laKhachVip)
        {
            if (DuocMienPhi(km, tongTien, laKhachVip)) return 0;
            return 30000;
        }

        public static string Chay()
        {
            return PhiGiaoHang(10, 500000, false) + "," + PhiGiaoHang(10, 3000000, false) + "," + PhiGiaoHang(1, 500000, false);
        }
    }

    // =====================================================================
    // 2. CONSOLIDATE DUPLICATE CONDITIONAL FRAGMENTS - đưa đoạn lặp ra ngoài nhánh if/else
    // =====================================================================
    public static class ConsolidateDuplicateFragments_Before
    {
        public static string Chay()
        {
            var nhatKy = new List<string>();
            long tongTien = 1500000;
            string ketQua;
            if (tongTien > 1000000)
            {
                ketQua = "Giam 5%";
                nhatKy.Add("Luu hoa don");
            }
            else
            {
                ketQua = "Khong giam";
                nhatKy.Add("Luu hoa don");
            }
            return ketQua + " | " + string.Join(",", nhatKy);
        }
    }

    public static class ConsolidateDuplicateFragments_After
    {
        public static string Chay()
        {
            var nhatKy = new List<string>();
            long tongTien = 1500000;
            string ketQua;
            if (tongTien > 1000000)
                ketQua = "Giam 5%";
            else
                ketQua = "Khong giam";

            nhatKy.Add("Luu hoa don"); // đoạn giống nhau ở cả hai nhánh được đưa ra ngoài
            return ketQua + " | " + string.Join(",", nhatKy);
        }
    }

    // =====================================================================
    // 3. DECOMPOSE CONDITIONAL - tách điều kiện phức tạp và các nhánh thành phương thức
    // =====================================================================
    public static class DecomposeConditional_Before
    {
        public static long TinhTien(DateTime ngay, int soLuong)
        {
            long phi;
            if (ngay >= new DateTime(ngay.Year, 6, 1) && ngay <= new DateTime(ngay.Year, 8, 31))
                phi = soLuong * 95000L;
            else
                phi = soLuong * 85000L + 20000;
            return phi;
        }

        public static string Chay()
        {
            return TinhTien(new DateTime(2026, 7, 10), 10) + "," + TinhTien(new DateTime(2026, 11, 10), 10);
        }
    }

    public static class DecomposeConditional_After
    {
        private static bool LaMuaVu(DateTime ngay)
        {
            return ngay >= new DateTime(ngay.Year, 6, 1) && ngay <= new DateTime(ngay.Year, 8, 31);
        }

        private static long GiaMuaVu(int soLuong) { return soLuong * 95000L; }

        private static long GiaThuong(int soLuong) { return soLuong * 85000L + 20000; }

        public static long TinhTien(DateTime ngay, int soLuong)
        {
            return LaMuaVu(ngay) ? GiaMuaVu(soLuong) : GiaThuong(soLuong);
        }

        public static string Chay()
        {
            return TinhTien(new DateTime(2026, 7, 10), 10) + "," + TinhTien(new DateTime(2026, 11, 10), 10);
        }
    }

    // =====================================================================
    // 4. REPLACE CONDITIONAL WITH POLYMORPHISM - switch theo loại => mỗi loại một lớp
    // =====================================================================
    public static class ReplaceConditionalWithPolymorphism_Before
    {
        public enum PhuongThuc { TienMat, ChuyenKhoan, The }

        public static long TinhPhi(PhuongThuc loai, long soTien)
        {
            switch (loai)
            {
                case PhuongThuc.TienMat: return 0;
                case PhuongThuc.ChuyenKhoan: return 11000;
                case PhuongThuc.The: return soTien * 2 / 100;
                default: throw new InvalidOperationException("Phuong thuc khong hop le");
            }
        }

        public static string Chay()
        {
            return TinhPhi(PhuongThuc.TienMat, 1000000) + "," + TinhPhi(PhuongThuc.ChuyenKhoan, 1000000) + "," + TinhPhi(PhuongThuc.The, 1000000);
        }
    }

    public static class ReplaceConditionalWithPolymorphism_After
    {
        public abstract class PhuongThucThanhToan
        {
            public abstract long TinhPhi(long soTien);
        }

        public class TienMat : PhuongThucThanhToan
        {
            public override long TinhPhi(long soTien) { return 0; }
        }

        public class ChuyenKhoan : PhuongThucThanhToan
        {
            public override long TinhPhi(long soTien) { return 11000; }
        }

        public class TheNganHang : PhuongThucThanhToan
        {
            public override long TinhPhi(long soTien) { return soTien * 2 / 100; }
        }

        public static string Chay()
        {
            return new TienMat().TinhPhi(1000000) + "," + new ChuyenKhoan().TinhPhi(1000000) + "," + new TheNganHang().TinhPhi(1000000);
        }
    }

    // =====================================================================
    // 5. REMOVE CONTROL FLAG - bỏ biến cờ điều khiển vòng lặp, dùng return/break
    // =====================================================================
    public static class RemoveControlFlag_Before
    {
        public static string TimSanPham(List<string> ds, string canTim)
        {
            bool timThay = false;
            string ketQua = "Khong co";
            foreach (var sp in ds)
            {
                if (!timThay)
                {
                    if (sp == canTim)
                    {
                        ketQua = "Co " + sp;
                        timThay = true;
                    }
                }
            }
            return ketQua;
        }

        public static string Chay()
        {
            var ds = new List<string> { "Regent 800WG", "Glyphosate", "Validamycin" };
            return TimSanPham(ds, "Glyphosate") + "," + TimSanPham(ds, "ABC");
        }
    }

    public static class RemoveControlFlag_After
    {
        public static string TimSanPham(List<string> ds, string canTim)
        {
            foreach (var sp in ds)
            {
                if (sp == canTim)
                    return "Co " + sp;
            }
            return "Khong co";
        }

        public static string Chay()
        {
            var ds = new List<string> { "Regent 800WG", "Glyphosate", "Validamycin" };
            return TimSanPham(ds, "Glyphosate") + "," + TimSanPham(ds, "ABC");
        }
    }

    // =====================================================================
    // 6. REPLACE NESTED CONDITIONAL WITH GUARD CLAUSES - if lồng nhau => thoát sớm
    // =====================================================================
    public static class ReplaceNestedWithGuardClauses_Before
    {
        public static long HoaHong(bool daNghiViec, bool dangThuViec, long doanhSo)
        {
            long ketQua;
            if (daNghiViec)
            {
                ketQua = 0;
            }
            else
            {
                if (dangThuViec)
                {
                    ketQua = 0;
                }
                else
                {
                    if (doanhSo < 10000000)
                        ketQua = doanhSo * 1 / 100;
                    else
                        ketQua = doanhSo * 3 / 100;
                }
            }
            return ketQua;
        }

        public static string Chay()
        {
            return HoaHong(true, false, 20000000) + "," + HoaHong(false, true, 20000000) + "," + HoaHong(false, false, 5000000) + "," + HoaHong(false, false, 20000000);
        }
    }

    public static class ReplaceNestedWithGuardClauses_After
    {
        public static long HoaHong(bool daNghiViec, bool dangThuViec, long doanhSo)
        {
            if (daNghiViec) return 0;
            if (dangThuViec) return 0;
            if (doanhSo < 10000000) return doanhSo * 1 / 100;
            return doanhSo * 3 / 100;
        }

        public static string Chay()
        {
            return HoaHong(true, false, 20000000) + "," + HoaHong(false, true, 20000000) + "," + HoaHong(false, false, 5000000) + "," + HoaHong(false, false, 20000000);
        }
    }

    // =====================================================================
    // 7. INTRODUCE NULL OBJECT - thay kiểm tra null rải rác bằng đối tượng "rỗng"
    // =====================================================================
    public static class IntroduceNullObject_Before
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public double TyLeGiam { get; set; }
        }

        private static readonly List<KhachHang> _ds = new List<KhachHang>
        {
            new KhachHang { Ten = "Anh Bay", TyLeGiam = 0.05 }
        };

        private static KhachHang TimKhach(string ten)
        {
            foreach (var k in _ds) if (k.Ten == ten) return k;
            return null;
        }

        public static string Chay()
        {
            var sb = new StringBuilder();
            foreach (var ten in new[] { "Anh Bay", "Nguoi la" })
            {
                KhachHang kh = TimKhach(ten);
                string hienThi = kh == null ? "Khach le" : kh.Ten;
                double giam = kh == null ? 0 : kh.TyLeGiam;
                sb.Append(hienThi + ":" + giam + ";");
            }
            return sb.ToString();
        }
    }

    public static class IntroduceNullObject_After
    {
        public class KhachHang
        {
            public virtual string Ten { get; set; }
            public virtual double TyLeGiam { get; set; }
        }

        // Đối tượng rỗng: có hành vi mặc định hợp lý, không cần kiểm tra null nữa
        public class KhachLe : KhachHang
        {
            public override string Ten { get { return "Khach le"; } set { } }
            public override double TyLeGiam { get { return 0; } set { } }
        }

        private static readonly List<KhachHang> _ds = new List<KhachHang>
        {
            new KhachHang { Ten = "Anh Bay", TyLeGiam = 0.05 }
        };

        private static KhachHang TimKhach(string ten)
        {
            foreach (var k in _ds) if (k.Ten == ten) return k;
            return new KhachLe();
        }

        public static string Chay()
        {
            var sb = new StringBuilder();
            foreach (var ten in new[] { "Anh Bay", "Nguoi la" })
            {
                KhachHang kh = TimKhach(ten);
                sb.Append(kh.Ten + ":" + kh.TyLeGiam + ";");
            }
            return sb.ToString();
        }
    }

    // =====================================================================
    // 8. INTRODUCE ASSERTION - ghi rõ điều kiện giả định bằng Assert thay vì chỉ comment
    // (Debug.Assert chỉ có hiệu lực khi chạy chế độ Debug)
    // =====================================================================
    public static class IntroduceAssertion_Before
    {
        public static long TinhThanhTien(int soLuong, long donGia)
        {
            // Giả định: soLuong phải lớn hơn 0 và donGia không âm
            return soLuong * donGia;
        }

        public static string Chay() { return "Thanh tien: " + TinhThanhTien(20, 85000); }
    }

    public static class IntroduceAssertion_After
    {
        public static long TinhThanhTien(int soLuong, long donGia)
        {
            Debug.Assert(soLuong > 0, "So luong phai lon hon 0");
            Debug.Assert(donGia >= 0, "Don gia khong duoc am");
            return soLuong * donGia;
        }

        public static string Chay() { return "Thanh tien: " + TinhThanhTien(20, 85000); }
    }

    // =====================================================================
    // CHƯƠNG TRÌNH CHẠY THỬ
    // =====================================================================
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Kiem("1. Consolidate Conditional Expression", ConsolidateConditional_Before.Chay(), ConsolidateConditional_After.Chay());
            Kiem("2. Consolidate Duplicate Conditional Fragments", ConsolidateDuplicateFragments_Before.Chay(), ConsolidateDuplicateFragments_After.Chay());
            Kiem("3. Decompose Conditional", DecomposeConditional_Before.Chay(), DecomposeConditional_After.Chay());
            Kiem("4. Replace Conditional with Polymorphism", ReplaceConditionalWithPolymorphism_Before.Chay(), ReplaceConditionalWithPolymorphism_After.Chay());
            Kiem("5. Remove Control Flag", RemoveControlFlag_Before.Chay(), RemoveControlFlag_After.Chay());
            Kiem("6. Replace Nested Conditional with Guard Clauses", ReplaceNestedWithGuardClauses_Before.Chay(), ReplaceNestedWithGuardClauses_After.Chay());
            Kiem("7. Introduce Null Object", IntroduceNullObject_Before.Chay(), IntroduceNullObject_After.Chay());
            Kiem("8. Introduce Assertion", IntroduceAssertion_Before.Chay(), IntroduceAssertion_After.Chay());

            Console.ReadKey();
        }

        static void Kiem(string tenKyThuat, string truoc, string sau)
        {
            Console.WriteLine("[" + (truoc == sau ? "OK  " : "LOI ") + "] " + tenKyThuat + "  ->  " + sau);
        }
    }
}
