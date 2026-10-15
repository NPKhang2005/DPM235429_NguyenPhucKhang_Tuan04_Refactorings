using System;
using System.Collections.Generic;
using System.Text;

// Nhóm 3: ORGANIZING DATA (refactoring.guru/refactoring/techniques/organizing-data) - 15 kỹ thuật
// C# 7.3. Mỗi kỹ thuật có <TênKỹThuật>_Before và <TênKỹThuật>_After, đều có hàm Chay().
// Hai kỹ thuật đầu (Change Value to Reference / Reference to Value) cố ý đổi cách so sánh đối tượng,
// nên Main đánh dấu "duKienKhac = true" cho hai kỹ thuật này.
namespace CuahangNongduoc.Refactoring.OrganizingData
{
    public class ChiTiet
    {
        public string Ten { get; set; }
        public int SoLuong { get; set; }
        public long DonGia { get; set; }
        public long ThanhTien { get { return (long)SoLuong * DonGia; } }
    }

    // =====================================================================
    // 1. CHANGE VALUE TO REFERENCE - nhiều bản sao giống nhau => dùng chung một đối tượng
    // Vấn đề: mỗi hóa đơn tự tạo một KhachHang riêng, cùng khách mà có nhiều bản sao.
    // =====================================================================
    public static class ChangeValueToReference_Before
    {
        public class KhachHang { public string Ten { get; set; } }

        public class HoaDon
        {
            public KhachHang Khach { get; private set; }
            public HoaDon(string tenKhach) { Khach = new KhachHang { Ten = tenKhach }; }
        }

        public static string Chay()
        {
            var a = new HoaDon("Anh Bay");
            var b = new HoaDon("Anh Bay");
            return "Cung mot doi tuong khach: " + ReferenceEquals(a.Khach, b.Khach);
        }
    }

    public static class ChangeValueToReference_After
    {
        public class KhachHang { public string Ten { get; set; } }

        // Kho lưu trữ: mỗi tên khách chỉ có đúng một đối tượng KhachHang
        public static class KhoKhachHang
        {
            private static readonly Dictionary<string, KhachHang> _ds = new Dictionary<string, KhachHang>();

            public static KhachHang Lay(string ten)
            {
                KhachHang kh;
                if (!_ds.TryGetValue(ten, out kh))
                {
                    kh = new KhachHang { Ten = ten };
                    _ds[ten] = kh;
                }
                return kh;
            }
        }

        public class HoaDon
        {
            public KhachHang Khach { get; private set; }
            public HoaDon(string tenKhach) { Khach = KhoKhachHang.Lay(tenKhach); }
        }

        public static string Chay()
        {
            var a = new HoaDon("Anh Bay");
            var b = new HoaDon("Anh Bay");
            return "Cung mot doi tuong khach: " + ReferenceEquals(a.Khach, b.Khach);
        }
    }

    // =====================================================================
    // 2. CHANGE REFERENCE TO VALUE - đối tượng nhỏ, bất biến, so sánh theo giá trị
    // Vấn đề: hai số tiền 100.000đ nhưng khác đối tượng nên Equals trả về false.
    // =====================================================================
    public static class ChangeReferenceToValue_Before
    {
        public class TienTe { public decimal SoTien { get; set; } }

        public static string Chay()
        {
            var a = new TienTe { SoTien = 100000 };
            var b = new TienTe { SoTien = 100000 };
            return "Hai so tien bang nhau: " + Equals(a, b);
        }
    }

    public static class ChangeReferenceToValue_After
    {
        public class TienTe
        {
            public decimal SoTien { get; private set; } // bất biến
            public TienTe(decimal soTien) { SoTien = soTien; }

            public override bool Equals(object obj)
            {
                var kia = obj as TienTe;
                return kia != null && kia.SoTien == SoTien;
            }

            public override int GetHashCode() { return SoTien.GetHashCode(); }
        }

        public static string Chay()
        {
            var a = new TienTe(100000);
            var b = new TienTe(100000);
            return "Hai so tien bang nhau: " + Equals(a, b);
        }
    }

    // =====================================================================
    // 3. DUPLICATE OBSERVED DATA - tách dữ liệu nghiệp vụ khỏi giao diện, đồng bộ bằng sự kiện
    // Vấn đề: logic tính thành tiền nằm ngay trong lớp giao diện, trên các ô nhập chữ.
    // =====================================================================
    public static class DuplicateObservedData_Before
    {
        public class FormBanHang
        {
            public string TxtDonGia, TxtSoLuong, TxtThanhTien;

            public void CapNhatThanhTien()
            {
                TxtThanhTien = ((long)long.Parse(TxtDonGia) * int.Parse(TxtSoLuong)).ToString();
            }
        }

        public static string Chay()
        {
            var f = new FormBanHang { TxtDonGia = "85000", TxtSoLuong = "20" };
            f.CapNhatThanhTien();
            return f.TxtThanhTien;
        }
    }

    public static class DuplicateObservedData_After
    {
        // Mô hình dữ liệu: giữ nghiệp vụ, phát sự kiện khi đổi
        public class ChiTietModel
        {
            private long _donGia;
            private int _soLuong;
            public event Action Changed;

            public long DonGia { get { return _donGia; } set { _donGia = value; Phat(); } }
            public int SoLuong { get { return _soLuong; } set { _soLuong = value; Phat(); } }
            public long ThanhTien { get { return _donGia * _soLuong; } }

            private void Phat() { if (Changed != null) Changed(); }
        }

        // Giao diện: chỉ giữ bản sao để hiển thị, tự cập nhật khi model đổi
        public class FormBanHang
        {
            public string TxtDonGia, TxtSoLuong, TxtThanhTien;
            private readonly ChiTietModel _model;

            public FormBanHang(ChiTietModel model)
            {
                _model = model;
                _model.Changed += CapNhatHienThi;
            }

            private void CapNhatHienThi()
            {
                TxtDonGia = _model.DonGia.ToString();
                TxtSoLuong = _model.SoLuong.ToString();
                TxtThanhTien = _model.ThanhTien.ToString();
            }

            public void NhapLieu(string donGia, string soLuong)
            {
                _model.DonGia = long.Parse(donGia);
                _model.SoLuong = int.Parse(soLuong);
            }
        }

        public static string Chay()
        {
            var f = new FormBanHang(new ChiTietModel());
            f.NhapLieu("85000", "20");
            return f.TxtThanhTien;
        }
    }

    // =====================================================================
    // 4. SELF ENCAPSULATE FIELD - trong lớp cũng truy cập trường qua thuộc tính
    // =====================================================================
    public static class SelfEncapsulateField_Before
    {
        public class SanPham
        {
            private long _donGia;
            public SanPham(long donGia) { _donGia = donGia; }
            public long GiaSi() { return _donGia * 90 / 100; }
        }

        public static string Chay() { return "Gia si: " + new SanPham(85000).GiaSi(); }
    }

    public static class SelfEncapsulateField_After
    {
        public class SanPham
        {
            private long _donGia;
            // Lớp con có thể override để đổi cách lấy giá mà không sửa GiaSi()
            protected virtual long DonGia { get { return _donGia; } set { _donGia = value; } }

            public SanPham(long donGia) { DonGia = donGia; }
            public long GiaSi() { return DonGia * 90 / 100; }
        }

        public static string Chay() { return "Gia si: " + new SanPham(85000).GiaSi(); }
    }

    // =====================================================================
    // 5. REPLACE DATA VALUE WITH OBJECT - dữ liệu thô có hành vi riêng => thành lớp
    // Vấn đề: số điện thoại là string, mỗi nơi tự định dạng.
    // =====================================================================
    public static class ReplaceDataValueWithObject_Before
    {
        public class KhachHang
        {
            public string Ten { get; set; }
            public string SoDienThoai { get; set; }
        }

        public static string Chay()
        {
            var kh = new KhachHang { Ten = "Anh Bay", SoDienThoai = "0918070557" };
            string so = kh.SoDienThoai;
            return kh.Ten + ": " + so.Substring(0, 4) + "." + so.Substring(4, 3) + "." + so.Substring(7);
        }
    }

    public static class ReplaceDataValueWithObject_After
    {
        public class SoDienThoai
        {
            private readonly string _so;

            public SoDienThoai(string so)
            {
                if (so == null || so.Length != 10) throw new ArgumentException("So dien thoai phai co 10 chu so");
                _so = so;
            }

            public string DinhDang() { return _so.Substring(0, 4) + "." + _so.Substring(4, 3) + "." + _so.Substring(7); }
        }

        public class KhachHang
        {
            public string Ten { get; set; }
            public SoDienThoai SoDienThoai { get; set; }
        }

        public static string Chay()
        {
            var kh = new KhachHang { Ten = "Anh Bay", SoDienThoai = new SoDienThoai("0918070557") };
            return kh.Ten + ": " + kh.SoDienThoai.DinhDang();
        }
    }

    // =====================================================================
    // 6. REPLACE ARRAY WITH OBJECT - mảng chứa dữ liệu khác loại => đối tượng có tên trường
    // =====================================================================
    public static class ReplaceArrayWithObject_Before
    {
        public static string Chay()
        {
            string[] dong = { "Regent 800WG", "20", "85000" }; // dong[0] là gì? dong[1] là gì?
            return dong[0] + ": " + dong[1] + " x " + dong[2];
        }
    }

    public static class ReplaceArrayWithObject_After
    {
        public class DongHang
        {
            public string Ten { get; set; }
            public int SoLuong { get; set; }
            public long DonGia { get; set; }
        }

        public static string Chay()
        {
            var dong = new DongHang { Ten = "Regent 800WG", SoLuong = 20, DonGia = 85000 };
            return dong.Ten + ": " + dong.SoLuong + " x " + dong.DonGia;
        }
    }

    // =====================================================================
    // 7. CHANGE UNIDIRECTIONAL ASSOCIATION TO BIDIRECTIONAL
    // Vấn đề: muốn biết khách có bao nhiêu hóa đơn phải quét toàn bộ danh sách hóa đơn.
    // =====================================================================
    public static class ChangeUniToBi_Before
    {
        public class KhachHang { public string Ten { get; set; } }
        public class HoaDon { public KhachHang Khach { get; set; } }

        public static int DemHoaDon(KhachHang khach, List<HoaDon> tatCa)
        {
            int dem = 0;
            foreach (var hd in tatCa) if (hd.Khach == khach) dem++;
            return dem;
        }

        public static string Chay()
        {
            var k = new KhachHang { Ten = "Anh Bay" };
            var tatCa = new List<HoaDon> { new HoaDon { Khach = k }, new HoaDon { Khach = k } };
            return "So hoa don: " + DemHoaDon(k, tatCa);
        }
    }

    public static class ChangeUniToBi_After
    {
        public class KhachHang
        {
            private readonly List<HoaDon> _hoaDon = new List<HoaDon>();
            public string Ten { get; set; }
            public int SoHoaDon { get { return _hoaDon.Count; } }
            internal void ThemHoaDon(HoaDon hd) { _hoaDon.Add(hd); }
        }

        public class HoaDon
        {
            public KhachHang Khach { get; private set; }

            // Cập nhật cả hai chiều trong một chỗ duy nhất
            public void DatKhach(KhachHang khach)
            {
                Khach = khach;
                khach.ThemHoaDon(this);
            }
        }

        public static string Chay()
        {
            var k = new KhachHang { Ten = "Anh Bay" };
            new HoaDon().DatKhach(k);
            new HoaDon().DatKhach(k);
            return "So hoa don: " + k.SoHoaDon;
        }
    }

    // =====================================================================
    // 8. CHANGE BIDIRECTIONAL ASSOCIATION TO UNIDIRECTIONAL
    // Vấn đề: ChiTiet giữ tham chiếu ngược về HoaDon chỉ để lấy tỉ lệ giảm giá.
    // =====================================================================
    public static class ChangeBiToUni_Before
    {
        public class HoaDon
        {
            public double TyLeGiam { get; set; }
            public List<ChiTietHD> DanhSach { get; private set; }
            public HoaDon() { DanhSach = new List<ChiTietHD>(); }
        }

        public class ChiTietHD
        {
            public HoaDon HoaDon { get; set; } // tham chiếu ngược
            public long ThanhTien { get; set; }
            public long SauGiam() { return (long)(ThanhTien * (1 - HoaDon.TyLeGiam)); }
        }

        public static string Chay()
        {
            var hd = new HoaDon { TyLeGiam = 0.1 };
            var ct = new ChiTietHD { HoaDon = hd, ThanhTien = 1700000 };
            hd.DanhSach.Add(ct);
            return "Sau giam: " + ct.SauGiam();
        }
    }

    public static class ChangeBiToUni_After
    {
        public class HoaDon
        {
            public double TyLeGiam { get; set; }
            public List<ChiTietHD> DanhSach { get; private set; }
            public HoaDon() { DanhSach = new List<ChiTietHD>(); }
        }

        public class ChiTietHD
        {
            public long ThanhTien { get; set; }
            // Tỉ lệ giảm được truyền vào khi cần, không giữ tham chiếu ngược
            public long SauGiam(double tyLeGiam) { return (long)(ThanhTien * (1 - tyLeGiam)); }
        }

        public static string Chay()
        {
            var hd = new HoaDon { TyLeGiam = 0.1 };
            var ct = new ChiTietHD { ThanhTien = 1700000 };
            hd.DanhSach.Add(ct);
            return "Sau giam: " + ct.SauGiam(hd.TyLeGiam);
        }
    }

    // =====================================================================
    // 9. ENCAPSULATE FIELD - trường public => private + thuộc tính
    // =====================================================================
    public static class EncapsulateField_Before
    {
        public class SanPham { public string Ten; }

        public static string Chay()
        {
            var sp = new SanPham();
            sp.Ten = "Regent 800WG";
            return sp.Ten;
        }
    }

    public static class EncapsulateField_After
    {
        public class SanPham
        {
            private string _ten;
            public string Ten
            {
                get { return _ten; }
                set { _ten = value; } // nơi có thể thêm kiểm tra dữ liệu sau này
            }
        }

        public static string Chay()
        {
            var sp = new SanPham();
            sp.Ten = "Regent 800WG";
            return sp.Ten;
        }
    }

    // =====================================================================
    // 10. ENCAPSULATE COLLECTION - không trả danh sách gốc cho bên ngoài tự sửa
    // =====================================================================
    public static class EncapsulateCollection_Before
    {
        public class HoaDon { public List<ChiTiet> DanhSach { get; set; } }

        public static string Chay()
        {
            var hd = new HoaDon { DanhSach = new List<ChiTiet>() };
            hd.DanhSach.Add(new ChiTiet { Ten = "Regent", SoLuong = 20, DonGia = 85000 });
            hd.DanhSach.Add(new ChiTiet { Ten = "NPK", SoLuong = 10, DonGia = 320000 });
            long tong = 0;
            foreach (var ct in hd.DanhSach) tong += ct.ThanhTien;
            return "Tong: " + tong;
        }
    }

    public static class EncapsulateCollection_After
    {
        public class HoaDon
        {
            private readonly List<ChiTiet> _danhSach = new List<ChiTiet>();

            // Bên ngoài chỉ được đọc
            public IEnumerable<ChiTiet> DanhSach { get { return _danhSach.AsReadOnly(); } }

            public void ThemChiTiet(ChiTiet ct) { _danhSach.Add(ct); }
            public void XoaChiTiet(ChiTiet ct) { _danhSach.Remove(ct); }
        }

        public static string Chay()
        {
            var hd = new HoaDon();
            hd.ThemChiTiet(new ChiTiet { Ten = "Regent", SoLuong = 20, DonGia = 85000 });
            hd.ThemChiTiet(new ChiTiet { Ten = "NPK", SoLuong = 10, DonGia = 320000 });
            long tong = 0;
            foreach (var ct in hd.DanhSach) tong += ct.ThanhTien;
            return "Tong: " + tong;
        }
    }

    // =====================================================================
    // 11. REPLACE MAGIC NUMBER WITH SYMBOLIC CONSTANT
    // =====================================================================
    public static class ReplaceMagicNumber_Before
    {
        public static long TinhVat(long thanhTien) { return thanhTien * 8 / 100; }
        public static string Chay() { return "VAT: " + TinhVat(1700000); }
    }

    public static class ReplaceMagicNumber_After
    {
        private const int TY_LE_VAT_PHAN_TRAM = 8;

        public static long TinhVat(long thanhTien) { return thanhTien * TY_LE_VAT_PHAN_TRAM / 100; }
        public static string Chay() { return "VAT: " + TinhVat(1700000); }
    }

    // =====================================================================
    // 12. REPLACE TYPE CODE WITH CLASS - mã loại là số nguyên => lớp có kiểm tra kiểu
    // =====================================================================
    public static class ReplaceTypeCodeWithClass_Before
    {
        public const int THUOC = 0;
        public const int PHAN_BON = 1;
        public const int HAT_GIONG = 2;

        public static string TenLoai(int loai)
        {
            switch (loai)
            {
                case THUOC: return "Thuoc BVTV";
                case PHAN_BON: return "Phan bon";
                case HAT_GIONG: return "Hat giong";
                default: return "Khong ro";
            }
        }

        public static string Chay() { return "Loai: " + TenLoai(PHAN_BON); }
    }

    public static class ReplaceTypeCodeWithClass_After
    {
        public class LoaiHang
        {
            public static readonly LoaiHang Thuoc = new LoaiHang("Thuoc BVTV");
            public static readonly LoaiHang PhanBon = new LoaiHang("Phan bon");
            public static readonly LoaiHang HatGiong = new LoaiHang("Hat giong");

            public string Ten { get; private set; }
            private LoaiHang(string ten) { Ten = ten; } // chỉ tạo được 3 giá trị ở trên
        }

        public static string Chay() { return "Loai: " + LoaiHang.PhanBon.Ten; }
    }

    // =====================================================================
    // 13. REPLACE TYPE CODE WITH SUBCLASSES - mã loại làm đổi hành vi => lớp con
    // =====================================================================
    public static class ReplaceTypeCodeWithSubclasses_Before
    {
        public class NhanVien
        {
            public const int BAN_HANG = 0;
            public const int QUAN_LY = 1;
            public const int THU_KHO = 2;

            private readonly int _loai;
            public NhanVien(int loai) { _loai = loai; }

            public long Luong()
            {
                switch (_loai)
                {
                    case BAN_HANG: return 6000000;
                    case QUAN_LY: return 12000000;
                    case THU_KHO: return 7000000;
                    default: throw new InvalidOperationException("Loai nhan vien khong hop le");
                }
            }
        }

        public static string Chay()
        {
            return new NhanVien(NhanVien.BAN_HANG).Luong() + "," + new NhanVien(NhanVien.QUAN_LY).Luong() + "," + new NhanVien(NhanVien.THU_KHO).Luong();
        }
    }

    public static class ReplaceTypeCodeWithSubclasses_After
    {
        public abstract class NhanVien { public abstract long Luong(); }
        public class NhanVienBanHang : NhanVien { public override long Luong() { return 6000000; } }
        public class QuanLy : NhanVien { public override long Luong() { return 12000000; } }
        public class ThuKho : NhanVien { public override long Luong() { return 7000000; } }

        public static string Chay()
        {
            return new NhanVienBanHang().Luong() + "," + new QuanLy().Luong() + "," + new ThuKho().Luong();
        }
    }

    // =====================================================================
    // 14. REPLACE TYPE CODE WITH STATE/STRATEGY - mã trạng thái đổi theo thời gian => đối tượng State
    // =====================================================================
    public static class ReplaceTypeCodeWithState_Before
    {
        public class HoaDon
        {
            public const int MOI = 0;
            public const int DA_THANH_TOAN = 1;
            public const int DA_GIAO = 2;

            public int TrangThai { get; set; }

            public bool CoTheHuy()
            {
                switch (TrangThai)
                {
                    case MOI: return true;
                    case DA_THANH_TOAN: return true;
                    default: return false;
                }
            }

            public string TenTrangThai()
            {
                switch (TrangThai)
                {
                    case MOI: return "Moi";
                    case DA_THANH_TOAN: return "Da thanh toan";
                    default: return "Da giao";
                }
            }
        }

        public static string Chay()
        {
            var a = new HoaDon { TrangThai = HoaDon.DA_THANH_TOAN };
            var b = new HoaDon { TrangThai = HoaDon.DA_GIAO };
            return a.TenTrangThai() + ":" + a.CoTheHuy() + "," + b.TenTrangThai() + ":" + b.CoTheHuy();
        }
    }

    public static class ReplaceTypeCodeWithState_After
    {
        public abstract class TrangThaiHoaDon
        {
            public abstract string Ten { get; }
            public abstract bool CoTheHuy { get; }
        }

        public class Moi : TrangThaiHoaDon
        {
            public override string Ten { get { return "Moi"; } }
            public override bool CoTheHuy { get { return true; } }
        }

        public class DaThanhToan : TrangThaiHoaDon
        {
            public override string Ten { get { return "Da thanh toan"; } }
            public override bool CoTheHuy { get { return true; } }
        }

        public class DaGiao : TrangThaiHoaDon
        {
            public override string Ten { get { return "Da giao"; } }
            public override bool CoTheHuy { get { return false; } }
        }

        public class HoaDon
        {
            public TrangThaiHoaDon TrangThai { get; set; }
            public bool CoTheHuy() { return TrangThai.CoTheHuy; }
            public string TenTrangThai() { return TrangThai.Ten; }
        }

        public static string Chay()
        {
            var a = new HoaDon { TrangThai = new DaThanhToan() };
            var b = new HoaDon { TrangThai = new DaGiao() };
            return a.TenTrangThai() + ":" + a.CoTheHuy() + "," + b.TenTrangThai() + ":" + b.CoTheHuy();
        }
    }

    // =====================================================================
    // 15. REPLACE SUBCLASS WITH FIELDS - các lớp con chỉ khác nhau hằng số => dùng trường
    // =====================================================================
    public static class ReplaceSubclassWithFields_Before
    {
        public abstract class SanPham { public abstract string NhomHang { get; } }
        public class ThuocTruSau : SanPham { public override string NhomHang { get { return "Thuoc tru sau"; } } }
        public class ThuocDietCo : SanPham { public override string NhomHang { get { return "Thuoc diet co"; } } }

        public static string Chay()
        {
            return new ThuocTruSau().NhomHang + "," + new ThuocDietCo().NhomHang;
        }
    }

    public static class ReplaceSubclassWithFields_After
    {
        public class SanPham
        {
            public string NhomHang { get; private set; }
            private SanPham(string nhomHang) { NhomHang = nhomHang; }

            public static SanPham TaoThuocTruSau() { return new SanPham("Thuoc tru sau"); }
            public static SanPham TaoThuocDietCo() { return new SanPham("Thuoc diet co"); }
        }

        public static string Chay()
        {
            return SanPham.TaoThuocTruSau().NhomHang + "," + SanPham.TaoThuocDietCo().NhomHang;
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

            // Hai kỹ thuật cố ý đổi cách so sánh đối tượng: kết quả cũ và mới PHẢI khác nhau
            Kiem("1. Change Value to Reference", ChangeValueToReference_Before.Chay(), ChangeValueToReference_After.Chay(), true);
            Kiem("2. Change Reference to Value", ChangeReferenceToValue_Before.Chay(), ChangeReferenceToValue_After.Chay(), true);

            Kiem("3. Duplicate Observed Data", DuplicateObservedData_Before.Chay(), DuplicateObservedData_After.Chay());
            Kiem("4. Self Encapsulate Field", SelfEncapsulateField_Before.Chay(), SelfEncapsulateField_After.Chay());
            Kiem("5. Replace Data Value with Object", ReplaceDataValueWithObject_Before.Chay(), ReplaceDataValueWithObject_After.Chay());
            Kiem("6. Replace Array with Object", ReplaceArrayWithObject_Before.Chay(), ReplaceArrayWithObject_After.Chay());
            Kiem("7. Change Unidirectional to Bidirectional", ChangeUniToBi_Before.Chay(), ChangeUniToBi_After.Chay());
            Kiem("8. Change Bidirectional to Unidirectional", ChangeBiToUni_Before.Chay(), ChangeBiToUni_After.Chay());
            Kiem("9. Encapsulate Field", EncapsulateField_Before.Chay(), EncapsulateField_After.Chay());
            Kiem("10. Encapsulate Collection", EncapsulateCollection_Before.Chay(), EncapsulateCollection_After.Chay());
            Kiem("11. Replace Magic Number with Symbolic Constant", ReplaceMagicNumber_Before.Chay(), ReplaceMagicNumber_After.Chay());
            Kiem("12. Replace Type Code with Class", ReplaceTypeCodeWithClass_Before.Chay(), ReplaceTypeCodeWithClass_After.Chay());
            Kiem("13. Replace Type Code with Subclasses", ReplaceTypeCodeWithSubclasses_Before.Chay(), ReplaceTypeCodeWithSubclasses_After.Chay());
            Kiem("14. Replace Type Code with State/Strategy", ReplaceTypeCodeWithState_Before.Chay(), ReplaceTypeCodeWithState_After.Chay());
            Kiem("15. Replace Subclass with Fields", ReplaceSubclassWithFields_Before.Chay(), ReplaceSubclassWithFields_After.Chay());

            Console.ReadKey();
        }

        static void Kiem(string tenKyThuat, string truoc, string sau, bool duKienKhac = false)
        {
            bool giong = truoc == sau;
            bool dat = giong != duKienKhac;
            Console.WriteLine("[" + (dat ? "OK  " : "LOI ") + "] " + tenKyThuat);
            Console.WriteLine("       Truoc: " + truoc);
            Console.WriteLine("       Sau  : " + sau);
        }
    }
}
