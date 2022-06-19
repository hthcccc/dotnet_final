﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using WebApplication2.Repository;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    /// <summary>
    /// 设置路由
    /// </summary>
    [Route("api/[Controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "customer")]
    public class CustomerController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static CustomerController instance = new CustomerController();
        /// <summary>对应类的实例</summary>
        public static CustomerController Instance { get => instance; set => instance = value; }

        
        public static Customer getCustomer(string customer_id)
        {
            using(var customerRepo=new CustomerRepository())
            {
                Customer customer = customerRepo.Customers.Find(customer_id);
                return customer;
            }
        }

        /// <summary>
        /// 返回指定用户信息
        /// </summary>
        /// <param name="customer_id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Customer), 200)]
        [HttpGet("{customer_id}")]
        public Dictionary<string,object> getCustomerInformation(string customer_id)
        {
            Customer customer = getCustomer(customer_id);
            if (customer != null)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                result.Add("customer_id", customer.customer_id);
                result.Add("customer_name", customer.customer_name);
                result.Add("birthday", customer.birthday.ToString().Replace(" 0:00:00",""));
                result.Add("phone_num", customer.phone);
                result.Add("credit", customer.credit);
                return result;
            }
            return null;
        }

        /// <summary>
        /// 返回积分
        /// </summary>
        /// <param name="customer_id"></param>
        /// <returns></returns>
        [HttpGet]
        public double? getCredit(string customer_id)
        {
            if (getCustomer(customer_id) != null)
            {
                return getCustomer(customer_id).credit; 
            }
            return null;
        }

        /// <summary>
        /// 顾客id密码校验
        /// </summary>
        /// <param name="customer_id"></param>
        ///<param name="pwd"></param>
        /// <returns></returns>
        /// <remarks>
        /// 0-id不存在 1-用户名密码匹配正确 2-用户名存在密码错误 3-数据库连接失败
        /// </remarks>
        [HttpGet]
        public ActionResult<int> checkCustomerAndPwd(string customer_id, string pwd)
        {
            var customerRepo = new CustomerRepository();
            Customer customer = null;
            customer = customerRepo.Customers.Find(customer_id);
            if (customer != null) 
            {
                if (pwd.Equals(customer.password)) return 1;
                else return 2;
            }
            return 0;

        }

        /// <summary>
        /// 创建新用户
        /// </summary>
        /// <param name="birthday"></param>
        /// <param name="customer_name"></param>
        /// <param name="phone_num"></param>
        /// <param name="customer_password"></param>
        /// <remarks>
        /// 创建失败:"创建失败"
        /// 新用户创建成功:新用户id
        /// </remarks>
        [HttpPost]
        public ActionResult<string> newCustomer(string phone_num, string customer_name, string customer_password, string birthday)
        {
            int yr = DateTime.Now.Year;
            int mon = DateTime.Now.Month;
            int day = DateTime.Now.Day;
            string date = yr.ToString().Substring(2, 2) + mon.ToString() + day.ToString();
            if (date.Length == 4)
            {
                date = date.Insert(2, "0");
                date = date.Insert(4, "0");
            }
            else if (date.Length == 5 && mon < 10)
            {
                date = date.Insert(2, "0");
            }
            else if (date.Length == 5 && day < 10)
            {
                date = date.Insert(4, "0");
            }
            string customer_id = date + "0000";
            var customerRepo = new CustomerRepository();
            if (customerRepo.Customers.Where(p => p.phone.Equals(phone_num)).Count() > 0) return "创建失败";

            //生成新id机制
            while (customerRepo.Customers.Find(customer_id) != null)
            {
                customer_id = customer_id.Substring(0, customer_id.Length - 4);
                for (int i = 0; i < 4; i++)
                {
                    customer_id += new Random().Next(0, 9).ToString();
                }
            }
            Customer customer = new Customer();
            customer.phone = phone_num;
            customer.password = customer_password;
            customer.customer_name = customer_name;
            customer.customer_id = customer_id;
            customer.credit = -1;
            customer.birthday = DateTime.Parse(birthday);
            customerRepo.Customers.Add(customer);
            customerRepo.SaveChanges();
            return customer_id;
        }

        /// <summary>
        /// 顾客信息修改
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// 0-id不存在 1-修改成功 2-手机号已存在
        /// </remarks>
        [HttpPost]
        public ActionResult<int> setCustomerInfo(string customer_id,string customer_name,string birthday,string phone_num)
        {
            var customerRepo = new CustomerRepository();
            Customer customer = null;
            customer = customerRepo.Customers.Find(customer_id);
            if (customer == null) return 0;
            if (customerRepo.Customers.Where(p => p.phone.Equals(phone_num)).Count() > 0) return 2;

            customer.phone = phone_num;
            customer.customer_name = customer_name;
            customer.birthday = DateTime.Parse(birthday);
            customerRepo.SaveChanges();
            return 1;

        }
    }
}
