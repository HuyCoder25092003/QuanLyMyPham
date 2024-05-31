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
    public class SanPhamSvc : GenericSvc<SanPhamRep, SanPham>
    {
        private SanPhamRep sanPhamRep;
        readonly QuanLyMyPhamContext context = new QuanLyMyPhamContext();
        public SanPhamSvc()
        {
            sanPhamRep = new SanPhamRep();
        }
        #region -- Overrides --

        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();
            if (id == 0)
                res.SetError("Can not find Product");
            else
            {
                var m = _rep.Read(id);
                if(m == null)
                    res.SetError("Can not find Product");
                res.Data = m;
            }
            return res;
        }
        //public override SingleRsp Remove(int id)
        //{
        //    var res = new SingleRsp();
        //    res.Data = _rep.Remove(id);
        //    return res;
        //}
        public override SingleRsp Update(SanPham m)
        {
            var res = new SingleRsp();

            var m1 = m.MaSp > 0 ? _rep.Read(m.MaSp) : _rep.Read(m.TenSp);
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
        #endregion
        public SingleRsp CreateProduct(SanPhamReq sanPhamReq)
        {
            var res = new SingleRsp();
            SanPham sanPham = new SanPham();
            if (context.SanPhams.Any(l=>l.TenSp == sanPhamReq.TenSp) || sanPhamReq.MaLoaiSp == null)
            {
                res.SetError("Can not add product");
                return res;
            }
            else
            {
                sanPham.TenSp = sanPhamReq.TenSp;
                sanPham.Gia = sanPhamReq.Gia;
                sanPham.HinhAnh = sanPhamReq.HinhAnh;
                sanPham.MaLoaiSp = sanPhamReq.MaLoaiSp;
                return res = sanPhamRep.CreateProduct(sanPham);
            }
            
        }

        public SingleRsp UpdateProduct(int Id,SanPhamReq sanPhamReq)
        {
            var res = new SingleRsp();
            var existingCustomer = sanPhamRep.Read(Id);
            if (existingCustomer == null)
            {
                res.SetError("Customer not found.");
            }
            else if (context.SanPhams.Any(l => l.TenSp == sanPhamReq.TenSp))
                res.SetError("Customer name is exit.");
            else
            {

                existingCustomer.TenSp = sanPhamReq.TenSp;
                existingCustomer.Gia = sanPhamReq.Gia;
                existingCustomer.HinhAnh = sanPhamReq.HinhAnh;
                res = sanPhamRep.UpdateProduct(existingCustomer);
            }
            return res;
        }
        public SingleRsp Remove(int id)
        {
            return sanPhamRep.Remove(id);

        }
        public SingleRsp TimKiem(string Keyword)
        {
            var res = new SingleRsp();
            var sanPhamsResponse = sanPhamRep.FindProduct(Keyword);

            if (sanPhamsResponse.Success)
            {
                var sanPhams = (List<SanPham>)sanPhamsResponse.Data;
                if (sanPhams.Count == 0)
                    res.SetError("No products found.");
                else
                    res.Data = sanPhams;
            }
            else
            {
                res.SetError(sanPhamsResponse.Message);
            }

            return res;
        }
        public SingleRsp SearchProduct(SearchProductReq searchProductReq)
        {
            var res = new SingleRsp();
            //lay dssp theo keyword
            var sanPhams = sanPhamRep.SearchProduct(searchProductReq.Keyword);
            //xu ly phan trang
            int pCount, totalPage, offset;
            offset = searchProductReq.Size * (searchProductReq.Page - 1);
            pCount = sanPhams.Count;
            totalPage = (pCount % searchProductReq.Size) == 0 ? pCount / searchProductReq.Size : 1 + (pCount / searchProductReq.Size);
            var p = new
            {
                Data = sanPhams.Skip(offset).Take(searchProductReq.Size).ToList(),
                Page = searchProductReq.Page,
                Size = searchProductReq.Size
            };
            res.Data = p;
            return res;
        }
        public SingleRsp SearchProductByCategoryName(string categoryName)
        {
            var res = new SingleRsp();
            var sanPhamsResponse = sanPhamRep.SearchProductByCategoryName(categoryName);

            if (sanPhamsResponse.Success)
            {
                var sanPhams = (List<SanPham>)sanPhamsResponse.Data;
                if (sanPhams.Count == 0)
                    res.SetError("No products found.");
                else
                    res.Data = sanPhams;
            }
            else
            {
                res.SetError(sanPhamsResponse.Message);
            }

            return res;
        }
        public SingleRsp SearchProductByPrice(double minPrice, double maxPrice)
        {
            var res = new SingleRsp();
            var sanPhams = sanPhamRep.SearchProductByPrice(minPrice, maxPrice);

            if (sanPhams == null || sanPhams.Count == 0)
            {
                res.SetError("No products found.");
            }
            else
            {
                res.Data = sanPhams;
            }

            return res;
        }

        public List<SanPham> GetAllProduct()
        {
            var sanPhams = sanPhamRep.GetAllProduct();
            return sanPhams;
        }
    }
}