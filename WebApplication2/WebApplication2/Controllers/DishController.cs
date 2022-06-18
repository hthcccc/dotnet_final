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
    [ApiExplorerSettings(GroupName = "dish")]
    public class DishController
    {
        /// <summary>用于各类之间接口的访问</summary>
        private static DishController instance = new DishController();
        /// <summary>对应类的实例</summary>
        public static DishController Instance { get => instance; set => instance = value; }

        /// <summary>
        /// 获取菜品类型
        /// </summary>
        /// <param name="dish_id"></param>
        /// <returns></returns>
        [HttpGet]
        public List<string> getDishType(string dish_id)
        {
            var typeRepo = new TypeRepository();
            return typeRepo.Types.Where(p => p.dish_id.Equals(dish_id)).Select(p => p.dish_type).ToList();
        }

        /// <summary>
        /// 返回菜单
        /// </summary>
        /// <returns></returns>
        [ProducesResponseType(typeof(List<Dish>), 200)]
        [HttpGet]
        public List<Dictionary<string,object>> getAllDish()
        {
            List<Dictionary<string, object>> result = new List<Dictionary<string, object>>();
            var dishRepo = new DishRepository();
            List<Dish> dishes = dishRepo.Dishes.ToList();
            foreach(var dish in dishes)
            {
                Dictionary<string, object> element = new Dictionary<string, object>();
                element.Add("dish_id", dish.dish_id);
                element.Add("dish_name", dish.name);
                element.Add("dish_price", dish.price);
                element.Add("dish_type", getDishType(dish.dish_id));
                element.Add("dish_rate", dish.count);
                element.Add("dish_info", dish.info);

                result.Add(element);
            }
            return result;
        }

        /// <summary>
        /// 返回某道菜品
        /// </summary>
        /// <param name="dish_id"></param>
        /// <returns></returns>
        [ProducesResponseType(typeof(Dish), 200)]
        [HttpGet("{dish_id}")]
        public Dish getDish(String dish_id)
        {
            using (var dishRepo = new DishRepository())
            {
                Dish dish = dishRepo.Dishes.Find(dish_id);
                return dish;
            }
        }
    }
}
