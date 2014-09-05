using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace Frosty_Sejuani
{
    class Program
    {
        public static HitChance test;
        public static string ChampName = "Sejuani";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Spell Q, W, E, R;

        public static Menu SejuaniWrapper;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
           // if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 650);
            W = new Spell(SpellSlot.W, 350);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R, 1175);
            Q.SetSkillshot(0.5f, 75.0f, 20.0f, true, SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 110.0f, 1600.0f, true, SkillshotType.SkillshotLine);

            SejuaniWrapper = new Menu(ChampName, ChampName, true);

            SejuaniWrapper.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(SejuaniWrapper.SubMenu("Orbwalker"));
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            SejuaniWrapper.AddSubMenu(ts);
            SejuaniWrapper.AddSubMenu(new Menu("Combo", "Combo"));
            SejuaniWrapper.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            SejuaniWrapper.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "COMBO!").SetValue(new KeyBind(32, KeyBindType.Press)));
            SejuaniWrapper.SubMenu("Combo").AddItem(new MenuItem("useRc", "Minimum R Hit").SetValue(new Slider(2, 1, 5)));
            SejuaniWrapper.SubMenu("Combo").AddItem(new MenuItem("chance", "Hitchance of R").SetValue(new Slider(2, 1, 4)));
            SejuaniWrapper.AddItem(new MenuItem("NFE", "Packet Casting").SetValue(true));
            SejuaniWrapper.AddSubMenu(new Menu("AntiGapclose", "AntiGapclose"));
            SejuaniWrapper.SubMenu("AntiGapclose").AddItem(new MenuItem("useqg", "Use Q to AntiGapclose").SetValue(true));
            SejuaniWrapper.AddSubMenu(new Menu("Interrupt", "Interrupt"));
            SejuaniWrapper.SubMenu("Interrupt").AddItem(new MenuItem("useq", "Use Q to Interrupt").SetValue(true));
            SejuaniWrapper.SubMenu("Interrupt").AddItem(new MenuItem("user", "Use R to Interrupt").SetValue(true));
            SejuaniWrapper.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; 
            Game.OnGameUpdate += Game_OnGameUpdate;
            AntiGapcloser.OnEnemyGapcloser += AntiGapclose;
            Game.PrintChat("Frosty " + ChampName + " by newchild");
            Interrupter.OnPosibleToInterrupt += OnPosibleToInterrupt;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (SejuaniWrapper.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.Azure);
            Utility.DrawCircle(Player.Position, W.Range, Color.Black);
            Utility.DrawCircle(Player.Position, E.Range, Color.Crimson);
            Utility.DrawCircle(Player.Position, R.Range, Color.Gold);
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, SejuaniWrapper.Item("NFE").GetValue<bool>());

            }
            if (target.IsValidTarget(Player.AttackRange) && W.IsReady())
            {
                W.Cast();
            }
            if (target.IsValidTarget(R.Range) && R.IsReady() && SejuaniWrapper.Item("useR").GetValue<bool>())
            {
                if(SejuaniWrapper.Item("chance").GetValue<Slider>().Value == 1){
                   test = HitChance.Low;
               }
                if(SejuaniWrapper.Item("chance").GetValue<Slider>().Value == 2){
                   test = HitChance.Medium;
               }        
                if(SejuaniWrapper.Item("chance").GetValue<Slider>().Value == 3){
                   test = HitChance.High;
               }
                if(SejuaniWrapper.Item("chance").GetValue<Slider>().Value == 4){
                   test = HitChance.Immobile;
               }
                R.CastIfHitchanceEquals(target, test, SejuaniWrapper.Item("NFE").GetValue<bool>());
                R.CastIfWillHit(target, SejuaniWrapper.Item("UseRc").GetValue<Slider>().Value, SejuaniWrapper.Item("NFE").GetValue<bool>());
            }
            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast();
            }
        }
        private static void OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (SejuaniWrapper.Item("useq").GetValue<bool>())
            {
                if (Player.Distance(target) < Q.Range && Q.IsReady())
                {
                    Q.Cast(target, SejuaniWrapper.Item("NFE").GetValue<bool>());
                }
            }
            if (SejuaniWrapper.Item("user").GetValue<bool>())
            {
                if (Player.Distance(target) < R.Range && R.IsReady())
                {
                    R.Cast(target, SejuaniWrapper.Item("NFE").GetValue<bool>());
                }
            }
        }
        public static void AntiGapclose(ActiveGapcloser gapcloser)
        {
            
            var target = gapcloser.End;
            if (SejuaniWrapper.Item("useqg").GetValue<bool>())
            {
               
                    Q.Cast(target, SejuaniWrapper.Item("NFE").GetValue<bool>());
                
            }
        }
    }
}
