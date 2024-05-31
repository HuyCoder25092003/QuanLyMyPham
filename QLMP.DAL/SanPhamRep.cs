using QLMP.Common.DAL;
using QLMP.Common.Rsp;
using QLMP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QLMP.DAL
{
    public class SanPhamRep : GenericRep<QuanLyMyPhamContext, SanPham>
    {
        #region -- Overrides --


        public override SanPham Read(int id)
        {
            var res = All.FirstOrDefault(p => p.MaSp == id);
            return res;
        }

        #endregion
       
        public SingleRsp CreateProduct(SanPham sanPham)
        {
            var res = new SingleRsp();
            using (var context = new QuanLyMyPhamContext())
            {
                using var tran = context.Database.BeginTransaction();
                try
                {
                    context.SanPhams.Add(sanPham);
                    context.SaveChanges();
                    tran.Commit();
                }
                catch (Exception ex)
                {
                    tran.Rollback();
                    res.SetError(ex.StackTrace);
                }
            }
            return res;
        }
        public SingleRsp UpdateProduct(SanPham sanPham)
        {
            var res = new SingleRsp();
            using (var context = new QuanLyMyPhamContext())
            {
                using var tran = context.Database.BeginTransaction();
                {
                    try
                    {
                        context.SanPhams.Update(sanPham);
                        context.SaveChanges();
                        tran.Commit();
                        res.Data = sanPham;
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        res.SetError(ex.StackTrace);
                    }
                }
               
            }
            return res;
        }
        public SingleRsp Remove(int Id)
        {
            var res = new SingleRsp();
            using (var context = new QuanLyMyPhamContext())
            {
                using var tran = context.Database.BeginTransaction();
                {
                    try
                    {
                        var sanPham = context.SanPhams.FirstOrDefault(s=>s.MaSp==Id);
                        if (sanPham != null)
                        {
                            context.SanPhams.Remove(sanPham);
                            context.SaveChanges();
                            tran.Commit();
                            res.Data = sanPham;
                        }
                        else
                        {
                            res.SetError("Product not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        res.SetError(ex.StackTrace);
                    }
                }
            }
            return res;
        }

        public List<SanPham> SearchProduct(string keyword)
        {
            return All.Where(x => x.TenSp.Contains(keyword)).ToList();
        }
        public SingleRsp FindProduct(string keyword)
        {
            var res = new SingleRsp();
            var products = All.Where(x => x.TenSp.Contains(keyword)).ToList();
            if(products == null)
                res.SetError("An error occurred while searching for products.");
            else
                res.Data = products;

            return res;
        }

        public SingleRsp SearchProductByCategoryName(string categoryName)
        { 
            var res = new SingleRsp();
            var products = All.Where(x => x.MaLoaiSpNavigation.TenLoaiSp.Contains(categoryName)).ToList();
            if (products == null)
                res.SetError("An error occurred while searching for products.");
            else
                res.Data = products;

            return res;
        }

        public List<SanPham> GetAllProduct()
        {
            return All.ToList();
        }
        public List<SanPham> SearchProductByPrice(double minPrice, double maxPrice)
        {
            var query = All.AsQueryable();

            if (minPrice > 0)
            {
                query = query.Where(x => x.Gia >= minPrice);
            }

            if (maxPrice > 0)
            {
                query = query.Where(x => x.Gia <= maxPrice);
            }

            return query.OrderBy(x => x.Gia).ToList();
        }

    }
}