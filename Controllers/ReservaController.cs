using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoPM.Models;

namespace ProyectoPM.Controllers
{
    public class ReservaController : Controller
    {
         
        private RestauranteContext _context;
        private SignInManager<Usuario> _sim;
        private UserManager<Usuario> _um;
        private RoleManager<IdentityRole> _rm;

        public ReservaController(
            RestauranteContext c, 
            SignInManager<Usuario> s,
            UserManager<Usuario> um,
            RoleManager<IdentityRole> rm){
            _context = c;
            _sim = s;
            _um = um;
            _rm = rm;
        }
        public IActionResult Index(int tipo)
        {
            var distrito =_context.Distritos.OrderByDescending(x=>x.Id).ToList();
            var sucursales = _context.Sucursales.Where(x=> x.Nombre==null).OrderByDescending(x=>x.Id).ToList();              
             if(tipo!=0){
                    sucursales = _context.Sucursales.Where(x=>x.DistritoId==tipo).ToList();
                }

            ViewBag.s = sucursales;
            ViewBag.d =distrito;          
           
            return View();
        }

        public IActionResult Detalles(int id)
        {
          var sucursalSelecta = _context.Sucursales.Where(x=>x.Id==id).ToList();
          ViewBag.se = sucursalSelecta;
          
          return View();
        }

        public IActionResult Reservar(int id)
        {
          var user = _um.FindByNameAsync(User.Identity.Name).Result;
          var sucursal = _context.Sucursales.Where(x=>x.Id==id).ToList();
          var sucursale = _context.Sucursales.FirstOrDefault(x=> x.Id==id);
          
            
          ViewBag.sucNom=sucursal;  
          ViewBag.Nmesa = sucursale.N_Mesas;
          ViewBag.IdentificadorSucursal = id;
          return View();
        }

        [HttpPost]
        public IActionResult Reservar(int SucursalId, Reserva r, int mesa, int horario, String fecha)
        {
          var user = _um.FindByNameAsync(User.Identity.Name).Result;
          var sucursal = _context.Sucursales.Where(x=>x.Id==SucursalId).ToList();
          var sucursale = _context.Sucursales.FirstOrDefault(x=> x.Id==SucursalId);
          var registroReservas = _context.Reservas.Where(x => x.SucursalId==SucursalId && x.Fecha==fecha && x.Mesa==mesa && x.Horario==horario).ToList();
        
          ViewBag.Nmesa = sucursale.N_Mesas;
          ViewBag.sucNom=sucursal;  
          ViewBag.Nmesa = sucursale.N_Mesas;
          ViewBag.IdentificadorSucursal = SucursalId;
          if (ModelState.IsValid)
          {
             if(registroReservas.Count==0){
                  r.Id = 0;
                  r.UserName = user.UserName;
                  _context.Add(r);
                  _context.SaveChanges(); 
                  return RedirectToAction("index", "home");
              }
              else{
                TempData["ocupado"] = "Mesa ya ocupada en ese horario";
                TempData["TipoTexto"] = "text-danger";
              }
              
          }   
          return View();              
        }

        public IActionResult MisReservas()
        {
          var user = _um.FindByNameAsync(User.Identity.Name).Result;
          var reservas = _context.Reservas.Where(x=> x.UserName==User.Identity.Name).ToList();           
          
          ViewBag.r = reservas;
          
          ViewBag.usuario = user.UserName;
          return View();
        }

        public IActionResult Reservas()
        {      
          var usuarios = _context.Usuarios.ToList();
          var reservas = _context.Reservas.Include(res => res.Sucursal).ToList();           
          
          ViewBag.r = reservas;
          ViewBag.u = usuarios; 




          return View();
        }

    }
}