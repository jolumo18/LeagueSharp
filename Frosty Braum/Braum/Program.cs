using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using System.Drawing;
using SharpDX;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Color = System.Drawing.Color;

namespace FrostyBraum
{
    class Program
    {
        public static HitChance test;
        public static string ChampName = "Braum";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Spell Q, W, E, R; 
        public static Menu BraumWrapper;
        public static Menu BraumAutomatic;
        
        static void Main(string[] args)
        {
                
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            
            

           if (Player.BaseSkinName != ChampName)
            {
              return;
           }

            Q = new Spell(SpellSlot.Q, 1000);
            W = new Spell(SpellSlot.W, 650);
            E = new Spell(SpellSlot.E,25000);
            R = new Spell(SpellSlot.R, 1250);
            Q.SetSkillshot(0.3333f, 70f, 1200f, true, LeagueSharp.Common.SkillshotType.SkillshotLine);
            R.SetSkillshot(0.5f, 80f, 1200f, false, LeagueSharp.Common.SkillshotType.SkillshotLine);
            BraumAutomatic = new Menu("Freaky Braum KS :^)", "Freaky Braum KS :^)");
            BraumWrapper = new Menu(ChampName, ChampName, true);
            BraumWrapper.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(BraumWrapper.SubMenu("Orbwalker"));
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            
            BraumWrapper.AddSubMenu(ts);
            BraumWrapper.AddSubMenu(new Menu("Combo", "Combo"));
            BraumWrapper.AddSubMenu(new Menu("Harrass", "Harrass"));
            BraumWrapper.SubMenu("Harrass").AddItem(new MenuItem("HarrassActive", "Harrass mode").SetValue(new KeyBind(16, KeyBindType.Toggle)));
            BraumWrapper.SubMenu("Harrass").AddItem(new MenuItem("HarrassMana", "Minimum Mana").SetValue(new Slider(60, 1, 100)));
            BraumWrapper.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R?").SetValue(true));
            BraumWrapper.SubMenu("Combo").AddItem(new MenuItem("useRc", "Minimum R Hit").SetValue(new Slider(2, 1, 5)));
            BraumWrapper.SubMenu("Combo").AddItem(new MenuItem("chance", "Hitchance of R").SetValue(new Slider(2, 1, 4)));
            BraumWrapper.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo").SetValue(new KeyBind(32, KeyBindType.Press)));
            BraumAutomatic.AddItem(new MenuItem("ksQ", "Use Q to ks").SetValue(true));
            BraumWrapper.AddItem(new MenuItem("NFE", "Packet Casting").SetValue(true));
            BraumWrapper.AddToMainMenu();
            Game.PrintChat("Frosty" + ChampName + " by newchild01");
            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

 
        }

        static void Game_OnGameUpdate(EventArgs args)
        {

            if (BraumWrapper.Item("HarrassActive").GetValue<KeyBind>().Active){
                Harrass();
            }
            if(BraumWrapper.Item("ksQ").GetValue<bool>()){
                ks();
            }
            
            if (BraumWrapper.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
        }
        static void ks(){
            foreach (
                var hero in
                ObjectManager.Get<Obj_AI_Hero>()
                    .Where(
                        hero =>
                            hero.IsValidTarget(Q.Range) &&
                            (Q.GetDamage(hero)) - 15 > hero.Health))
                Q.Cast();
}
        }
        static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.Azure);
            Utility.DrawCircle(Player.Position, W.Range, Color.Black);
            Utility.DrawCircle(Player.Position, E.Range, Color.Crimson);
            Utility.DrawCircle(Player.Position, R.Range, Color.Gold);
            Utility.DrawCircle(Player.Position, Player.ExpGiveRadius, Color.Green);
        }
        public static void Harrass()
        {
            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target.IsValidTarget(Q.Range) && Q.IsReady() && (100 * (Player.Mana / Player.MaxMana)) > BraumWrapper.Item("HarrassMana").GetValue<Slider>().Value)
            {
                Q.CastIfHitchanceEquals(target, HitChance.High, BraumWrapper.Item("NFE").GetValue<bool>());

            }
        }
        public static void Combo()
        {

            var target = SimpleTs.GetTarget(Q.Range, SimpleTs.DamageType.Magical);
            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, BraumWrapper.Item("NFE").GetValue<bool>());

            }
            if (target.IsValidTarget(R.Range) && R.IsReady())
            {
                
               if(BraumWrapper.Item("chance").GetValue<Slider>().Value == 1){
                   test = HitChance.Low;
               }
                if(BraumWrapper.Item("chance").GetValue<Slider>().Value == 2){
                   test = HitChance.Medium;
               }        
                if(BraumWrapper.Item("chance").GetValue<Slider>().Value == 3){
                   test = HitChance.High;
               }
                if(BraumWrapper.Item("chance").GetValue<Slider>().Value == 4){
                   test = HitChance.Immobile;
               }
                
                R.CastIfHitchanceEquals(target,test , BraumWrapper.Item("NFE").GetValue<bool>());
                R.CastIfWillHit(target, BraumWrapper.Item("UseRc").GetValue<Slider>().Value, BraumWrapper.Item("NFE").GetValue<bool>());
            }

            

            if (target.IsValidTarget(target.AttackRange) && W.IsReady())
            {
                W.Cast(Player, BraumWrapper.Item("NFE").GetValue<bool>());
            }
            
        }
    }
}
