using System;
using System.Collections.Generic;
using System.Text;

// Nhóm 5: SIMPLIFYING METHOD CALLS (refactoring.guru/refactoring/techniques/simplifying-method-calls) - 14 kỹ thuật
// C# 7.3. Mỗi kỹ thuật có <TênKỹThuật>_Before và <TênKỹThuật>_After, đều có hàm Chay().
namespace CuahangNongduoc.Refactoring.MethodCalls
{
    public class ChiTiet
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public long DonGia { get; set; }
    }

    // =====================================================================
    // 1. ADD PARAMETER - thêm tham số khi phương thức cần thêm thông tin
    // Ở đây thêm tỉ lệ giảm giá; gọi với 0 thì kết quả không đổi.
    // =====================================================================
    public static class AddParameter_Before
    {
        public static long TinhTien(int soLuong, long donGia) { return soLuong * donGia; }
        public static string Chay() { return "Tien: " + TinhTien(20, 85000); }
    }

    public static class AddParameter_After
    {
        public static long TinhTien(int soLuong, long donGia, double tyLeGiam)
        {
            return (long)Math.Round(soLuong * donGia * (1 - tyLeGiam));
        }

        public static string Chay() { return "Tien: " + TinhTien(20, 85000, 0); }
    }

    // =====================================================================
    // 2. REMOVE PARAMETER - bỏ tham số không còn được dùng
    // =====================================================================
    public static class RemoveParameter_Before
    {
        public static long TinhVat(long thanhTien, string ghiChu) { return thanhTien * 8 / 100; }
        public static string Chay() { return "VAT: " + TinhVat(1700000, "khong dung"); }
    }

    public static class RemoveParameter_After
    {
        public static long TinhVat(long thanhTien) { return thanhTien * 8 / 100; }
        public static string Chay() { return "VAT: " + TinhVat(1700000); }
    }

    // =====================================================================
    // 3. RENAME METHOD - đổi tên phương thức cho nói rõ việc nó làm
    // =====================================================================
    public static class RenameMethod_Before
    {
        public static long Tinh(long thanhTien) { return thanhTien + thanhTien * 8 / 100; }
        public static string Chay() { return "Tong: " + Tinh(1700000); }
    }

    public static class RenameMethod_After
    {
        public static long TinhTongTienSauVat(long thanhTien) { return thanhTien + thanhTien * 8 / 100; }
        public static string Chay() { return "Tong: " + TinhTongTienSauVat(1700000); }
    }

    // =====================================================================
    // 4. SEPARATE QUERY FROM MODIFIER - tách hàm vừa trả giá trị vừa đổi trạng thái
    // =====================================================================
    public static class SeparateQueryFromModifier_Before
    {
        public class Kho
        {
            private int _ton = 100;
            // Vừa trừ kho (thay đổi) vừa trả về tồn (truy vấn)
            public int TruKhoVaLayTon(int soLuong) { _ton -= soLuong; return _ton; }
        }

        public static string Chay()
        {
            var kho = new Kho();
            return "Ton: " + kho.TruKhoVaLayTon(30);
        }
    }

    public static class SeparateQueryFromModifier_After
    {
        public class Kho
        {
            private int _ton = 100;
            public int Ton { get { return _ton; } }          // chỉ truy vấn
            public void TruKho(int soLuong) { _ton -= soLuong; } // chỉ thay đổi
        }

        public static string Chay()
        {
            var kho = new Kho();
            kho.TruKho(30);
            return "Ton: " + kho.Ton;
        }
    }

    // =====================================================================
    // 5. PARAMETERIZE METHOD - gộp các phương thức giống nhau thành một có tham số
    // =====================================================================
    public static class ParameterizeMethod_Before
    {
        public static long TangGia5PhanTram(long gia) { return gia * 105 / 100; }
        public static long TangGia10PhanTram(long gia) { return gia * 110 / 100; }

        public static string Chay() { return TangGia5PhanTram(85000) + "," + TangGia10PhanTram(85000); }
    }

    public static class ParameterizeMethod_After
    {
        public static long TangGia(long gia, int phanTram) { return gia * (100 + phanTram) / 100; }

        public static string Chay() { return TangGia(85000, 5) + "," + TangGia(85000, 10); }
    }

    // =====================================================================
    // 6. INTRODUCE PARAMETER OBJECT - nhóm tham số hay đi cùng nhau thành một đối tượng
    // =====================================================================
    public static class IntroduceParameterObject_Before
    {
        public static int SoNgay(DateTime tu, DateTime den) { return (int)(den - tu).TotalDays; }
        public static bool HopLe(DateTime tu, DateTime den) { return tu <= den; }

        public static string Chay()
        {
            var tu = new DateTime(2026, 10, 1);
            var den = new DateTime(2026, 10, 15);
            return HopLe(tu, den) + "," + SoNgay(tu, den);
        }
    }

    public static class IntroduceParameterObject_After
    {
        public class KhoangNgay
        {
            public DateTime Tu { get; private set; }
            public DateTime Den { get; private set; }
            public KhoangNgay(DateTime tu, DateTime den) { Tu = tu; Den = den; }

            public int SoNgay() { return (int)(Den - Tu).TotalDays; }
            public bool HopLe() { return Tu <= Den; }
        }

        public static string Chay()
        {
            var kn = new KhoangNgay(new DateTime(2026, 10, 1), new DateTime(2026, 10, 15));
            return kn.HopLe() + "," + kn.SoNgay();
        }
    }

    // =====================================================================
    // 7. PRESERVE WHOLE OBJECT - truyền cả đối tượng thay vì tách từng giá trị
    // =====================================================================
    public static class PreserveWholeObject_Before
    {
        public static long TinhTien(int soLuong, long donGia) { return soLuong * donGia; }

        public static string Chay()
        {
            var ct = new ChiTiet { Ten = "Regent", SoLuong = 20, DonGia = 85000 };
            return "Tien: " + TinhTien(ct.SoLuong, ct.DonGia);
        }
    }

    public static class PreserveWholeObject_After
    {
        public static long TinhTien(ChiTiet ct) { return ct.SoLuong * ct.DonGia; }

        public static string Chay()
        {
            var ct = new ChiTiet { Ten = "Regent", SoLuong = 20, DonGia = 85000 };
            return "Tien: " + TinhTien(ct);
        }
    }

    // =====================================================================
    // 8. REMOVE SETTING METHOD - trường chỉ gán lúc tạo thì bỏ setter
    // =====================================================================
    public static class RemoveSettingMethod_Before
    {
        public class SanPham
        {
            public string MaSanPham { get; set; } // ai cũng đổi được mã
            public string Ten { get; set; }
        }

        public static string Chay()
        {
            var sp = new SanPham { MaSanPham = "NDA-001", Ten = "Regent 800WG" };
            return sp.MaSanPham + " - " + sp.Ten;
        }
    }

    public static class RemoveSettingMethod_After
    {
        public class SanPham
        {
            public string MaSanPham { get; private set; } // chỉ gán trong hàm tạo
            public string Ten { get; set; }

            public SanPham(string maSanPham) { MaSanPham = maSanPham; }
        }

        public static string Chay()
        {
            var sp = new SanPham("NDA-001") { Ten = "Regent 800WG" };
            return sp.MaSanPham + " - " + sp.Ten;
        }
    }

    // =====================================================================
    // 9. REPLACE PARAMETER WITH EXPLICIT METHODS - tham số điều khiển nhánh => phương thức riêng
    // =====================================================================
    public static class ReplaceParameterWithExplicitMethods_Before
    {
        public class SanPham
        {
            public long GiaBan { get; private set; }
            public long GiaNhap { get; private set; }

            public void Dat(string ten, long giaTri)
            {
                if (ten == "giaBan") GiaBan = giaTri;
                else if (ten == "giaNhap") GiaNhap = giaTri;
                else throw new ArgumentException("Ten khong hop le");
            }
        }

        public static string Chay()
        {
            var sp = new SanPham();
            sp.Dat("giaBan", 85000);
            sp.Dat("giaNhap", 70000);
            return sp.GiaBan + "," + sp.GiaNhap;
        }
    }

    public static class ReplaceParameterWithExplicitMethods_After
    {
        public class SanPham
        {
            public long GiaBan { get; private set; }
            public long GiaNhap { get; private set; }

            public void DatGiaBan(long giaTri) { GiaBan = giaTri; }
            public void DatGiaNhap(long giaTri) { GiaNhap = giaTri; }
        }

        public static string Chay()
        {
            var sp = new SanPham();
            sp.DatGiaBan(85000);
            sp.DatGiaNhap(70000);
            return sp.GiaBan + "," + sp.GiaNhap;
        }
    }

    // =====================================================================
    // 10. REPLACE PARAMETER WITH METHOD CALL - bỏ tham số có thể tự tính trong phương thức
    // =====================================================================
    public static class ReplaceParameterWithMethodCall_Before
    {
        private static double TyLeGiam(int soLuong) { return soLuong >= 20 ? 0.1 : 0; }

        public static long TinhTien(int soLuong, long donGia, double tyLeGiam)
        {
            return (long)Math.Round(soLuong * donGia * (1 - tyLeGiam));
        }

        public static string Chay()
        {
            int sl = 20;
            return "Tien: " + TinhTien(sl, 85000, TyLeGiam(sl)); // nơi gọi phải tự tính tỉ lệ giảm
        }
    }

    public static class ReplaceParameterWithMethodCall_After
    {
        private static double TyLeGiam(int soLuong) { return soLuong >= 20 ? 0.1 : 0; }

        public static long TinhTien(int soLuong, long donGia)
        {
            return (long)Math.Round(soLuong * donGia * (1 - TyLeGiam(soLuong)));
        }

        public static string Chay()
        {
            return "Tien: " + TinhTien(20, 85000);
        }
    }

    // =====================================================================
    // 11. HIDE METHOD - phương thức chỉ dùng nội bộ thì để private
    // =====================================================================
    public static class HideMethod_Before
    {
        public static long LamTronNghin(long so) { return (so + 500) / 1000 * 1000; } // public dù chỉ dùng nội bộ

        public static long TinhGiaBan(long giaNhap) { return LamTronNghin(giaNhap * 112 / 100); }

        public static string Chay() { return "Gia ban: " + TinhGiaBan(73333); }
    }

    public static class HideMethod_After
    {
        private static long LamTronNghin(long so) { return (so + 500) / 1000 * 1000; }

        public static long TinhGiaBan(long giaNhap) { return LamTronNghin(giaNhap * 112 / 100); }

        public static string Chay() { return "Gia ban: " + TinhGiaBan(73333); }
    }

    // =====================================================================
    // 12. REPLACE CONSTRUCTOR WITH FACTORY METHOD - hàm tạo có mã loại => phương thức tạo có tên
    // =====================================================================
    public static class ReplaceConstructorWithFactory_Before
    {
        public class SanPham
        {
            public int Loai { get; private set; } // 0: thuốc, 1: phân bón
            public string Ten { get; private set; }
            public SanPham(int loai, string ten) { Loai = loai; Ten = ten; }
        }

        public static string Chay()
        {
            var a = new SanPham(0, "Regent 800WG"); // 0 là gì? phải đoán
            var b = new SanPham(1, "NPK 20-20-15");
            return a.Loai + ":" + a.Ten + "," + b.Loai + ":" + b.Ten;
        }
    }

    public static class ReplaceConstructorWithFactory_After
    {
        public class SanPham
        {
            public int Loai { get; private set; }
            public string Ten { get; private set; }
            private SanPham(int loai, string ten) { Loai = loai; Ten = ten; }

            public static SanPham TaoThuoc(string ten) { return new SanPham(0, ten); }
            public static SanPham TaoPhanBon(string ten) { return new SanPham(1, ten); }
        }

        public static string Chay()
        {
            var a = SanPham.TaoThuoc("Regent 800WG");
            var b = SanPham.TaoPhanBon("NPK 20-20-15");
            return a.Loai + ":" + a.Ten + "," + b.Loai + ":" + b.Ten;
        }
    }

    // =====================================================================
    // 13. REPLACE ERROR CODE WITH EXCEPTION - mã lỗi trả về => ném ngoại lệ
    // =====================================================================
    public static class ReplaceErrorCodeWithException_Before
    {
        public class Kho
        {
            private int _ton = 100;

            // Trả về -1 nếu không đủ hàng: người gọi dễ quên kiểm tra
            public int RutKho(int soLuong)
            {
                if (soLuong > _ton) return -1;
                _ton -= soLuong;
                return _ton;
            }
        }

        public static string Chay()
        {
            var kho = new Kho();
            int kq = kho.RutKho(150);
            return kq == -1 ? "Het hang" : "Con " + kq;
        }
    }

    public static class ReplaceErrorCodeWithException_After
    {
        public class ThieuHangException : Exception
        {
            public ThieuHangException(string message) : base(message) { }
        }

        public class Kho
        {
            private int _ton = 100;

            public int RutKho(int soLuong)
            {
                if (soLuong > _ton) throw new ThieuHangException("Khong du hang trong kho");
                _ton -= soLuong;
                return _ton;
            }
        }

        public static string Chay()
        {
            var kho = new Kho();
            try
            {
                return "Con " + kho.RutKho(150);
            }
            catch (ThieuHangException)
            {
                return "Het hang";
            }
        }
    }

    // =====================================================================
    // 14. REPLACE EXCEPTION WITH TEST - tình huống đoán trước được thì kiểm tra trước, đừng bắt ngoại lệ
    // =====================================================================
    public static class ReplaceExceptionWithTest_Before
    {
        public static string LayTenSanPham(string[] ds, int viTri)
        {
            try
            {
                return ds[viTri];
            }
            catch (IndexOutOfRangeException)
            {
                return "Khong co";
            }
        }

        public static string Chay()
        {
            var ds = new[] { "Regent 800WG", "Glyphosate" };
            return LayTenSanPham(ds, 1) + "," + LayTenSanPham(ds, 5);
        }
    }

    public static class ReplaceExceptionWithTest_After
    {
        public static string LayTenSanPham(string[] ds, int viTri)
        {
            if (viTri < 0 || viTri >= ds.Length) return "Khong co";
            return ds[viTri];
        }

        public static string Chay()
        {
            var ds = new[] { "Regent 800WG", "Glyphosate" };
            return LayTenSanPham(ds, 1) + "," + LayTenSanPham(ds, 5);
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

            Kiem("1. Add Parameter", AddParameter_Before.Chay(), AddParameter_After.Chay());
            Kiem("2. Remove Parameter", RemoveParameter_Before.Chay(), RemoveParameter_After.Chay());
            Kiem("3. Rename Method", RenameMethod_Before.Chay(), RenameMethod_After.Chay());
            Kiem("4. Separate Query from Modifier", SeparateQueryFromModifier_Before.Chay(), SeparateQueryFromModifier_After.Chay());
            Kiem("5. Parameterize Method", ParameterizeMethod_Before.Chay(), ParameterizeMethod_After.Chay());
            Kiem("6. Introduce Parameter Object", IntroduceParameterObject_Before.Chay(), IntroduceParameterObject_After.Chay());
            Kiem("7. Preserve Whole Object", PreserveWholeObject_Before.Chay(), PreserveWholeObject_After.Chay());
            Kiem("8. Remove Setting Method", RemoveSettingMethod_Before.Chay(), RemoveSettingMethod_After.Chay());
            Kiem("9. Replace Parameter with Explicit Methods", ReplaceParameterWithExplicitMethods_Before.Chay(), ReplaceParameterWithExplicitMethods_After.Chay());
            Kiem("10. Replace Parameter with Method Call", ReplaceParameterWithMethodCall_Before.Chay(), ReplaceParameterWithMethodCall_After.Chay());
            Kiem("11. Hide Method", HideMethod_Before.Chay(), HideMethod_After.Chay());
            Kiem("12. Replace Constructor with Factory Method", ReplaceConstructorWithFactory_Before.Chay(), ReplaceConstructorWithFactory_After.Chay());
            Kiem("13. Replace Error Code with Exception", ReplaceErrorCodeWithException_Before.Chay(), ReplaceErrorCodeWithException_After.Chay());
            Kiem("14. Replace Exception with Test", ReplaceExceptionWithTest_Before.Chay(), ReplaceExceptionWithTest_After.Chay());

            Console.ReadKey();
        }

        static void Kiem(string tenKyThuat, string truoc, string sau)
        {
            Console.WriteLine("[" + (truoc == sau ? "OK  " : "LOI ") + "] " + tenKyThuat + "  ->  " + sau);
        }
    }
}
