using QLMP.Common.BLL;
using QLMP.Common.Req;
using QLMP.Common.Rsp;
using QLMP.DAL;
using QLMP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QLMP.BLL
{
    public class LoaiSpSvc : GenericSvc<LoaiSpRep, LoaiSanPham>
    {
        private LoaiSpRep loaiSpRep;
        private readonly QuanLyMyPhamContext context = new QuanLyMyPhamContext();
        public LoaiSpSvc()
        {
            loaiSpRep = new LoaiSpRep();
        }
        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();
            if (id == 0)
            {
                res.SetError("Cannot find category");
                return res;
            }
            var category = loaiSpRep.Read(id);
            if (category == null)
                res.SetError("Category not found");
            else
                res.Data = category;

            return res;
        }
        public override SingleRsp Update(LoaiSanPham m)
        {
            var res = new SingleRsp();

            var m1 = m.MaLoaiSp > 0 ? _rep.Read(m.MaLoaiSp) : _rep.Read(m.TenLoaiSp);
            if (m1 == null)
            {
                res.SetError("EZ103", "No data.");
            }
            else
            {
                res = base.Update(m);
                res.Data = m;
            }

            return res;
        }
        public SingleRsp Remove(int id)
        {

            return _rep.Remove(id);

        }
        public SingleRsp CreateCategory(LoaiSpReq loaiSpReq)
        {
            var res = new SingleRsp();
          
            LoaiSanPham l = new LoaiSanPham();
            if (string.IsNullOrEmpty(loaiSpReq.TenLoaiSp) || context.LoaiSanPhams.Any(l1 => l1.TenLoaiSp == loaiSpReq.TenLoaiSp))
                res.SetError("Can not add category");
            else
            {
                l.TenLoaiSp = loaiSpReq.TenLoaiSp;
                res = loaiSpRep.CreateCategory(l);
            }
            return res;
        }
        public SingleRsp UpdateCategory(int Id, LoaiSpReq loaiSpReq)
        {
            var res = new SingleRsp();
            var existingCategory = loaiSpRep.Read(Id);

            // Kiểm tra nếu category không tồn tại, tên loại sản phẩm rỗng hoặc chỉ chứa khoảng trắng, hoặc tên loại sản phẩm đã tồn tại
            if (existingCategory == null || string.IsNullOrWhiteSpace(loaiSpReq.TenLoaiSp) || context.LoaiSanPhams.Any(l => l.TenLoaiSp == loaiSpReq.TenLoaiSp))
            {
                res.SetError("Category edit fail.");
            }
            else
            {
                existingCategory.TenLoaiSp = loaiSpReq.TenLoaiSp;
                res = loaiSpRep.UpdateCategory(existingCategory);
            }
            return res;
        }

        public SingleRsp SearchCategory(SearchCateByName searchCateByName)
        {
            var res = new SingleRsp();
            if (string.IsNullOrEmpty(searchCateByName.Keyword))
                res.SetError("Category can not find");
            else
            {
                //lay dssp theo keyword
                var cates = loaiSpRep.SearchCategory(searchCateByName.Keyword);
                res.Data = cates;
            }
            return res;
        }
    }

}
