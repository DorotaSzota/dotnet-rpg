global using dotnet_rpg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_rpg.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class ChaacterController : ControllerBase   
    {
        private static List<Character> characters = new()
        {
            new Character(),
            new Character() {Id =1, Name = "Sam"}
        };
//ale burdel
        [HttpGet("GetAll")]
        public ActionResult<List<Character>> Get() {
            return Ok(characters);
        }

        [HttpGet("{id}")]
        public ActionResult<Character> GetSingle(int id) {
            return Ok(characters.FirstOrDefault(c => c.Id == id));
        }    








    }
}