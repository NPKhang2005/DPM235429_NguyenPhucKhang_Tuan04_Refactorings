using System;
using System.Collections.Generic;
using System.Text;

// Nhóm 6: DEALING WITH GENERALIZATION (refactoring.guru/refactoring/techniques/dealing-with-generalization) - 12 kỹ thuật
// C# 7.3. Mỗi kỹ thuật có <TênKỹThuật>_Before và <TênKỹThuật>_After, đều có hàm Chay().
namespace CuahangNongduoc.Refactoring.Generalization
{
    public class ChiTiet
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public long DonGia { get; set; }
        public long ThanhTien { get { return (long)SoLuong * DonGia; } }
    }

    // =====================================================================
    // 1. PULL UP FIELD - trường giống nhau ở các lớp con => đưa lên lớp cha
    // =====================================================================
    public static class PullUpField_Before
    {
        public abstract class DoiTac { }
        public class KhachHang : DoiTac { public string Ten { get; set; } }
        public class NhaCungCap : DoiTac { public string Ten { get; set; } }

        public static string Chay()
        {
            return new KhachHang { Ten = "Anh Bay" }.Ten + "," + new NhaCungCap { Ten = "Cong ty A" }.Ten;
        }
    }

    public static class PullUpField_After
    {
        public abstract class DoiTac { public string Ten { get; set; } }
        public class KhachHang : DoiTac { }
        public class NhaCungCap : DoiTac { }

        public static string Chay()
        {
            return new KhachHang { Ten = "Anh Bay" }.Ten + "," + new NhaCungCap { Ten = "Cong ty A" }.Ten;
        }
    }

    // =====================================================================
    // 2. PULL UP METHOD - phương thức giống nhau ở các lớp con => đưa lên lớp cha
    // =====================================================================
    public static class PullUpMethod_Before
    {
        public abstract class DoiTac { public string Ten { get; set; } }

        public class KhachHang : DoiTac
        {
            public string TenVietHoa() { return Ten.ToUpper(); }
        }

        public class NhaCungCap : DoiTac
        {
            public string TenVietHoa() { return Ten.ToUpper(); }
        }

        public static string Chay()
        {
            return new KhachHang { Ten = "Anh Bay" }.TenVietHoa() + "," + new NhaCungCap { Ten = "Cong ty A" }.TenVietHoa();
        }
    }

    public static class PullUpMethod_After
    {
        public abstract class DoiTac
        {
            public string Ten { get; set; }
            public string TenVietHoa() { return Ten.ToUpper(); }
        }

        public class KhachHang : DoiTac { }
        public class NhaCungCap : DoiTac { }

        public static string Chay()
        {
            return new KhachHang { Ten = "Anh Bay" }.TenVietHoa() + "," + new NhaCungCap { Ten = "Cong ty A" }.TenVietHoa();
        }
    }

    // =====================================================================
    // 3. PULL UP CONSTRUCTOR BODY - phần gán giá trị giống nhau trong hàm tạo => đưa lên hàm tạo lớp cha
    // =====================================================================
    public static class PullUpConstructorBody_Before
    {
        public abstract class DoiTac
        {
            public string Ma { get; protected set; }
            public string Ten { get; protected set; }
        }

        public class KhachHang : DoiTac
        {
            public double TyLeGiam { get; private set; }
            public KhachHang(string ma, string ten, double tyLeGiam)
            {
                Ma = ma;
                Ten = ten;
                TyLeGiam = tyLeGiam;
            }
        }

        public class NhaCungCap : DoiTac
        {
            public string DiaChi { get; private set; }
            public NhaCungCap(string ma, string ten, string diaChi)
            {
                Ma = ma;
                Ten = ten;
                DiaChi = diaChi;
            }
        }

        public static string Chay()
        {
            var k = new KhachHang("KH01", "Anh Bay", 0.05);
            var n = new NhaCungCap("NCC01", "Cong ty A", "Long Xuyen");
            return k.Ma + ":" + k.Ten + ":" + k.TyLeGiam + "," + n.Ma + ":" + n.Ten + ":" + n.DiaChi;
        }
    }

    public static class PullUpConstructorBody_After
    {
        public abstract class DoiTac
        {
            public string Ma { get; private set; }
            public string Ten { get; private set; }

            protected DoiTac(string ma, string ten)
            {
                Ma = ma;
                Ten = ten;
            }
        }

        public class KhachHang : DoiTac
        {
            public double TyLeGiam { get; private set; }
            public KhachHang(string ma, string ten, double tyLeGiam) : base(ma, ten)
            {
                TyLeGiam = tyLeGiam;
            }
        }

        public class NhaCungCap : DoiTac
        {
            public string DiaChi { get; private set; }
            public NhaCungCap(string ma, string ten, string diaChi) : base(ma, ten)
            {
                DiaChi = diaChi;
            }
        }

        public static string Chay()
        {
            var k = new KhachHang("KH01", "Anh Bay", 0.05);
            var n = new NhaCungCap("NCC01", "Cong ty A", "Long Xuyen");
            return k.Ma + ":" + k.Ten + ":" + k.TyLeGiam + "," + n.Ma + ":" + n.Ten + ":" + n.DiaChi;
        }
    }

    // =====================================================================
    // 4. PUSH DOWN METHOD - phương thức chỉ một lớp con dùng => đưa xuống lớp con đó
    // =====================================================================
    public static class PushDownMethod_Before
    {
        public abstract class SanPham
        {
            public string Ten { get; set; }
            // Chỉ thuốc mới có hạn dùng tính theo tháng, phân bón thì không
            public DateTime NgayHetHan(DateTime ngayNhap, int soThang) { return ngayNhap.AddMonths(soThang); }
        }

        public class Thuoc : SanPham { }
        public class PhanBon : SanPham { }

        public static string Chay()
        {
            var t = new Thuoc { Ten = "Regent 800WG" };
            return t.Ten + ": " + t.NgayHetHan(new DateTime(2026, 10, 15), 24).ToString("dd/MM/yyyy");
        }
    }

    public static class PushDownMethod_After
    {
        public abstract class SanPham { public string Ten { get; set; } }

        public class Thuoc : SanPham
        {
            public DateTime NgayHetHan(DateTime ngayNhap, int soThang) { return ngayNhap.AddMonths(soThang); }
        }

        public class PhanBon : SanPham { }

        public static string Chay()
        {
            var t = new Thuoc { Ten = "Regent 800WG" };
            return t.Ten + ": " + t.NgayHetHan(new DateTime(2026, 10, 15), 24).ToString("dd/MM/yyyy");
        }
    }

    // =====================================================================
    // 5. PUSH DOWN FIELD - trường chỉ một lớp con dùng => đưa xuống lớp con đó
    // =====================================================================
    public static class PushDownField_Before
    {
        public abstract class SanPham
        {
            public string Ten { get; set; }
            public string HoatChat { get; set; } // phân bón và hạt giống không dùng
        }

        public class Thuoc : SanPham { }
        public class HatGiong : SanPham { }

        public static string Chay()
        {
            var t = new Thuoc { Ten = "Regent 800WG", HoatChat = "Fipronil" };
            return t.Ten + " - " + t.HoatChat;
        }
    }

    public static class PushDownField_After
    {
        public abstract class SanPham { public string Ten { get; set; } }

        public class Thuoc : SanPham { public string HoatChat { get; set; } }
        public class HatGiong : SanPham { }

        public static string Chay()
        {
            var t = new Thuoc { Ten = "Regent 800WG", HoatChat = "Fipronil" };
            return t.Ten + " - " + t.HoatChat;
        }
    }

    // =====================================================================
    // 6. EXTRACT SUBCLASS - một lớp có tính năng chỉ dùng cho vài trường hợp => tách lớp con
    // =====================================================================
    public static class ExtractSubclass_Before
    {
        public class HoaDon
        {
            public long TongTien { get; set; }
            public bool CoGiaoHang { get; set; }
            public string DiaChiGiao { get; set; }
            public long PhiVanChuyen { get; set; }

            public long TongThanhToan() { return TongTien + (CoGiaoHang ? PhiVanChuyen : 0); }
        }

        public static string Chay()
        {
            var a = new HoaDon { TongTien = 1700000, CoGiaoHang = true, DiaChiGiao = "Long Xuyen", PhiVanChuyen = 30000 };
            var b = new HoaDon { TongTien = 1700000 };
            return a.TongThanhToan() + "," + b.TongThanhToan();
        }
    }

    public static class ExtractSubclass_After
    {
        public class HoaDon
        {
            public long TongTien { get; set; }
            public virtual long TongThanhToan() { return TongTien; }
        }

        public class HoaDonGiaoHang : HoaDon
        {
            public string DiaChiGiao { get; set; }
            public long PhiVanChuyen { get; set; }
            public override long TongThanhToan() { return TongTien + PhiVanChuyen; }
        }

        public static string Chay()
        {
            var a = new HoaDonGiaoHang { TongTien = 1700000, DiaChiGiao = "Long Xuyen", PhiVanChuyen = 30000 };
            var b = new HoaDon { TongTien = 1700000 };
            return a.TongThanhToan() + "," + b.TongThanhToan();
        }
    }

    // =====================================================================
    // 7. EXTRACT SUPERCLASS - hai lớp có phần giống nhau nhưng chưa có lớp cha chung => tạo lớp cha
    // =====================================================================
    public static class ExtractSuperclass_Before
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public string DiaChi { get; set; }
            public string ThongTin() { return Ten + " (" + DiaChi + ")"; }
        }

        public class NhaCungCap
        {
            public string Ten { get; set; }
            public string DiaChi { get; set; }
            public string ThongTin() { return Ten + " (" + DiaChi + ")"; }
        }

        public static string Chay()
        {
            return new KhachHang { Ten = "Anh Bay", DiaChi = "Long Xuyen" }.ThongTin() + "," +
                   new NhaCungCap { Ten = "Cong ty A", DiaChi = "Can Tho" }.ThongTin();
        }
    }

    public static class ExtractSuperclass_After
    {
        public abstract class DoiTac
        {
            public string Ten { get; set; }
            public string DiaChi { get; set; }
            public string ThongTin() { return Ten + " (" + DiaChi + ")"; }
        }

        public class KhachHang : DoiTac { }
        public class NhaCungCap : DoiTac { }

        public static string Chay()
        {
            return new KhachHang { Ten = "Anh Bay", DiaChi = "Long Xuyen" }.ThongTin() + "," +
                   new NhaCungCap { Ten = "Cong ty A", DiaChi = "Can Tho" }.ThongTin();
        }
    }

    // =====================================================================
    // 8. EXTRACT INTERFACE - nhiều lớp cùng có một nhóm thao tác => tách thành interface
    // =====================================================================
    public static class ExtractInterface_Before
    {
        public class HoaDon { public string NoiDungIn() { return "In hoa don ban hang"; } }
        public class PhieuNhap { public string NoiDungIn() { return "In phieu nhap kho"; } }

        // Mỗi loại phiếu một hàm riêng
        public static string In(HoaDon hd) { return hd.NoiDungIn(); }
        public static string In(PhieuNhap pn) { return pn.NoiDungIn(); }

        public static string Chay() { return In(new HoaDon()) + "," + In(new PhieuNhap()); }
    }

    public static class ExtractInterface_After
    {
        public interface IInDuoc { string NoiDungIn(); }

        public class HoaDon : IInDuoc { public string NoiDungIn() { return "In hoa don ban hang"; } }
        public class PhieuNhap : IInDuoc { public string NoiDungIn() { return "In phieu nhap kho"; } }

        // Một hàm dùng chung cho mọi thứ in được
        public static string In(IInDuoc phieu) { return phieu.NoiDungIn(); }

        public static string Chay() { return In(new HoaDon()) + "," + In(new PhieuNhap()); }
    }

    // =====================================================================
    // 9. COLLAPSE HIERARCHY - lớp con gần như không khác lớp cha => gộp lại
    // =====================================================================
    public static class CollapseHierarchy_Before
    {
        public class SanPham
        {
            public string Ten { get; set; }
            public long DonGia { get; set; }
        }

        public class SanPhamBan : SanPham { } // không thêm gì đáng kể

        public static string Chay()
        {
            var sp = new SanPhamBan { Ten = "Regent 800WG", DonGia = 85000 };
            return sp.Ten + ": " + sp.DonGia;
        }
    }

    public static class CollapseHierarchy_After
    {
        public class SanPham
        {
            public string Ten { get; set; }
            public long DonGia { get; set; }
        }

        public static string Chay()
        {
            var sp = new SanPham { Ten = "Regent 800WG", DonGia = 85000 };
            return sp.Ten + ": " + sp.DonGia;
        }
    }

    // =====================================================================
    // 10. FORM TEMPLATE METHOD - hai phương thức cùng khung, khác vài bước => khuôn mẫu ở lớp cha
    // =====================================================================
    public static class FormTemplateMethod_Before
    {
        public class BaoCaoNgay
        {
            public string InBaoCao(long[] doanhThu)
            {
                var sb = new StringBuilder();
                sb.Append("BAO CAO NGAY;");
                long tong = 0;
                foreach (var d in doanhThu) tong += d;
                sb.Append("Tong: " + tong + ";");
                sb.Append("Trung binh: " + tong / doanhThu.Length + ";");
                return sb.ToString();
            }
        }

        public class BaoCaoThang
        {
            public string InBaoCao(long[] doanhThu)
            {
                var sb = new StringBuilder();
                sb.Append("BAO CAO THANG;");
                long tong = 0;
                foreach (var d in doanhThu) tong += d;
                sb.Append("Tong: " + tong + ";");
                long max = 0;
                foreach (var d in doanhThu) if (d > max) max = d;
                sb.Append("Cao nhat: " + max + ";");
                return sb.ToString();
            }
        }

        public static string Chay()
        {
            var d = new long[] { 1000000, 2500000, 1500000 };
            return new BaoCaoNgay().InBaoCao(d) + "|" + new BaoCaoThang().InBaoCao(d);
        }
    }

    public static class FormTemplateMethod_After
    {
        public abstract class BaoCao
        {
            // Khuôn mẫu: thứ tự các bước cố định
            public string InBaoCao(long[] doanhThu)
            {
                var sb = new StringBuilder();
                sb.Append(TieuDe() + ";");
                long tong = 0;
                foreach (var d in doanhThu) tong += d;
                sb.Append("Tong: " + tong + ";");
                sb.Append(ThongKeRieng(doanhThu, tong) + ";");
                return sb.ToString();
            }

            protected abstract string TieuDe();
            protected abstract string ThongKeRieng(long[] doanhThu, long tong);
        }

        public class BaoCaoNgay : BaoCao
        {
            protected override string TieuDe() { return "BAO CAO NGAY"; }
            protected override string ThongKeRieng(long[] doanhThu, long tong) { return "Trung binh: " + tong / doanhThu.Length; }
        }

        public class BaoCaoThang : BaoCao
        {
            protected override string TieuDe() { return "BAO CAO THANG"; }
            protected override string ThongKeRieng(long[] doanhThu, long tong)
            {
                long max = 0;
                foreach (var d in doanhThu) if (d > max) max = d;
                return "Cao nhat: " + max;
            }
        }

        public static string Chay()
        {
            var d = new long[] { 1000000, 2500000, 1500000 };
            return new BaoCaoNgay().InBaoCao(d) + "|" + new BaoCaoThang().InBaoCao(d);
        }
    }

    // =====================================================================
    // 11. REPLACE INHERITANCE WITH DELEGATION - kế thừa mà chỉ dùng một phần => dùng thành phần bên trong
    // Vấn đề: GioHang kế thừa List<ChiTiet> nên lộ cả Insert, Sort, Clear... không cần thiết.
    // =====================================================================
    public static class ReplaceInheritanceWithDelegation_Before
    {
        public class GioHang : List<ChiTiet>
        {
            public long TongTien()
            {
                long tong = 0;
                foreach (var ct in this) tong += ct.ThanhTien;
                return tong;
            }
        }

        public static string Chay()
        {
            var gio = new GioHang();
            gio.Add(new ChiTiet { Ten = "Regent", SoLuong = 20, DonGia = 85000 });
            gio.Add(new ChiTiet { Ten = "NPK", SoLuong = 10, DonGia = 320000 });
            return "Tong: " + gio.TongTien();
        }
    }

    public static class ReplaceInheritanceWithDelegation_After
    {
        public class GioHang
        {
            private readonly List<ChiTiet> _danhSach = new List<ChiTiet>(); // thành phần bên trong, không lộ ra ngoài

            public void Them(ChiTiet ct) { _danhSach.Add(ct); }

            public long TongTien()
            {
                long tong = 0;
                foreach (var ct in _danhSach) tong += ct.ThanhTien;
                return tong;
            }
        }

        public static string Chay()
        {
            var gio = new GioHang();
            gio.Them(new ChiTiet { Ten = "Regent", SoLuong = 20, DonGia = 85000 });
            gio.Them(new ChiTiet { Ten = "NPK", SoLuong = 10, DonGia = 320000 });
            return "Tong: " + gio.TongTien();
        }
    }

    // =====================================================================
    // 12. REPLACE DELEGATION WITH INHERITANCE - ủy quyền gần như toàn bộ => kế thừa
    // =====================================================================
    public static class ReplaceDelegationWithInheritance_Before
    {
        public class NhanVien
        {
            public string Ten { get; set; }
            public long Luong { get; set; }
        }

        public class NhanVienBanHang
        {
            private readonly NhanVien _nhanVien;
            public NhanVienBanHang(NhanVien nv) { _nhanVien = nv; }

            public string Ten { get { return _nhanVien.Ten; } }
            public long Luong { get { return _nhanVien.Luong; } }
            public long TongThuNhap(long hoaHong) { return Luong + hoaHong; }
        }

        public static string Chay()
        {
            var nv = new NhanVienBanHang(new NhanVien { Ten = "Lan", Luong = 6000000 });
            return nv.Ten + ": " + nv.TongThuNhap(500000);
        }
    }

    public static class ReplaceDelegationWithInheritance_After
    {
        public class NhanVien
        {
            public string Ten { get; set; }
            public long Luong { get; set; }
        }

        public class NhanVienBanHang : NhanVien
        {
            public long TongThuNhap(long hoaHong) { return Luong + hoaHong; }
        }

        public static string Chay()
        {
            var nv = new NhanVienBanHang { Ten = "Lan", Luong = 6000000 };
            return nv.Ten + ": " + nv.TongThuNhap(500000);
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

            Kiem("1. Pull Up Field", PullUpField_Before.Chay(), PullUpField_After.Chay());
            Kiem("2. Pull Up Method", PullUpMethod_Before.Chay(), PullUpMethod_After.Chay());
            Kiem("3. Pull Up Constructor Body", PullUpConstructorBody_Before.Chay(), PullUpConstructorBody_After.Chay());
            Kiem("4. Push Down Method", PushDownMethod_Before.Chay(), PushDownMethod_After.Chay());
            Kiem("5. Push Down Field", PushDownField_Before.Chay(), PushDownField_After.Chay());
            Kiem("6. Extract Subclass", ExtractSubclass_Before.Chay(), ExtractSubclass_After.Chay());
            Kiem("7. Extract Superclass", ExtractSuperclass_Before.Chay(), ExtractSuperclass_After.Chay());
            Kiem("8. Extract Interface", ExtractInterface_Before.Chay(), ExtractInterface_After.Chay());
            Kiem("9. Collapse Hierarchy", CollapseHierarchy_Before.Chay(), CollapseHierarchy_After.Chay());
            Kiem("10. Form Template Method", FormTemplateMethod_Before.Chay(), FormTemplateMethod_After.Chay());
            Kiem("11. Replace Inheritance with Delegation", ReplaceInheritanceWithDelegation_Before.Chay(), ReplaceInheritanceWithDelegation_After.Chay());
            Kiem("12. Replace Delegation with Inheritance", ReplaceDelegationWithInheritance_Before.Chay(), ReplaceDelegationWithInheritance_After.Chay());

            Console.ReadKey();
        }

        static void Kiem(string tenKyThuat, string truoc, string sau)
        {
            Console.WriteLine("[" + (truoc == sau ? "OK  " : "LOI ") + "] " + tenKyThuat + "  ->  " + sau);
        }
    }
}
