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
        public static string ChampName = "Teemo";
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
           if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 580);
        
            SejuaniWrapper = new Menu(ChampName, ChampName, true);

            SejuaniWrapper.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(SejuaniWrapper.SubMenu("Orbwalker"));
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            SejuaniWrapper.AddSubMenu(ts);
            SejuaniWrapper.AddSubMenu(new Menu("Combo", "Combo"));
            SejuaniWrapper.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Cancer PLS!").SetValue(new KeyBind(32, KeyBindType.Press)));
            SejuaniWrapper.AddItem(new MenuItem("NFE", "Packet Casting").SetValue(true));
            SejuaniWrapper.AddSubMenu(new Menu("Interrupt", "Interrupt"));
            SejuaniWrapper.SubMenu("Interrupt").AddItem(new MenuItem("useq", "Use Q to Interrupt").SetValue(true));
            SejuaniWrapper.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw; 
            Game.OnGameUpdate += Game_OnGameUpdate; 

            Game.PrintChat("Cancer " + ChampName + " by newchild");
            Interrupter.OnPosibleToInterrupt += OnPosibleToInterrupt;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (SejuaniWrapper.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            ks();
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.Azure);
            Utility.DrawCircle(Player.Position, Player.AttackRange, Color.Azure);
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(R.Range, SimpleTs.DamageType.Magical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.CastOnUnit(target);

            }
        
        }
        private static void OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (SejuaniWrapper.Item("useq").GetValue<bool>())
            {
                if (Player.Distance(target) < Q.Range && Q.IsReady())
                {
                    Q.CastOnUnit(target);
                }
            }
        }
        static void ks(){
            foreach (
            var hero in
            ObjectManager.Get<Obj_AI_Hero>()
                .Where(
                    hero =>
                        hero.IsValidTarget(Q.Range) &&
                            (Q.GetDamage(hero)) > hero.Health))
                                Q.CastOnUnit(hero);
}
}
}

