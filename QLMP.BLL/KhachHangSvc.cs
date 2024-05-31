﻿using QLMP.Common.BLL;
using QLMP.Common.Rsp;
using QLMP.Common.Req;
using QLMP.DAL;
using QLMP.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace QLMP.BLL
{
    public class KhachHangSvc : GenericSvc<KhachHangRep, KhachHang>
    {
        private KhachHangRep khachHangRep;
        public KhachHangSvc()
        {
            khachHangRep = new KhachHangRep();
        }

        public override SingleRsp Read(int id)
        {
            var res = new SingleRsp();
            res.Data = _rep.Read(id);
            return res;
        }
       
        public override SingleRsp Update(KhachHang m)
        {
            var res = new SingleRsp();

            var m1 = m.MaKh > 0 ? _rep.Read(m.MaKh) : null;
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
        public SingleRsp Search(int userId)
        {
            var res = new SingleRsp();
            res= _rep.Search(userId);
            return res;
        }
       public SingleRsp UpdateCustomer(int Id, KhachHangReq khachHangReq)
        {
            var res = new SingleRsp();
            if (Id == 0)
            {
                res.SetError("Customer not found");
            }
            else
            {
                var existingCustomer = khachHangRep.Read(Id);
                if (existingCustomer == null)
                {
                    res.SetError("Customer not found.");
                }
                else
                {
                    // Kiểm tra từng trường thông tin và giữ nguyên giá trị hiện tại nếu để trống
                    existingCustomer.TenKh = string.IsNullOrEmpty(khachHangReq.TenKh) ? existingCustomer.TenKh : khachHangReq.TenKh;
                    existingCustomer.DiaChi = string.IsNullOrEmpty(khachHangReq.DiaChi) ? existingCustomer.DiaChi : khachHangReq.DiaChi;
                    existingCustomer.Email = string.IsNullOrEmpty(khachHangReq.Email) ? existingCustomer.Email : khachHangReq.Email;
                    existingCustomer.Sdt = string.IsNullOrEmpty((khachHangReq.Sdt).ToString()) ? existingCustomer.Sdt : khachHangReq.Sdt;

                    res = khachHangRep.UpdateCustomer(existingCustomer);
                }
            }
            return res;
        }


        public SingleRsp SearchCustomer(string keyword)
        {
            var res = new SingleRsp();
            var cates = khachHangRep.SearchCustomer(keyword);
            res.Data = cates;
            return res;
        }
    }
}
