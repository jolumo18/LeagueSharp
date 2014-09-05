using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;
using Color = System.Drawing.Color;
namespace Frosty_Yorick
{
    class Program
    {
        public static string ChampName = "Yorick";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player; // Instead of typing ObjectManager.Player you can just type Player
        public static Spell Q, W, E, R;

        public static Menu YorickWrapper;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;

            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 650);
            R = new Spell(SpellSlot.R, 900);
            W.SetSkillshot(0.6f, 0f, 20f, false, SkillshotType.SkillshotCircle);

            YorickWrapper = new Menu(ChampName, ChampName, true);

            YorickWrapper.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(YorickWrapper.SubMenu("Orbwalker"));

            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            YorickWrapper.AddSubMenu(ts);
            YorickWrapper.AddSubMenu(new Menu("Combo", "Combo"));
            YorickWrapper.AddSubMenu(new Menu("Harrass", "Harrass"));
            YorickWrapper.SubMenu("Harrass").AddItem(new MenuItem("HarrassActive", "Harrass!").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
            YorickWrapper.SubMenu("Harrass").AddItem(new MenuItem("UseWH", "Use W in Harrass").SetValue(true));
            YorickWrapper.SubMenu("Harrass").AddItem(new MenuItem("UseEH", "Use E in Harrass").SetValue(true));
            YorickWrapper.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R at Health:").SetValue(new Slider(5, 0, 100)));
            YorickWrapper.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));

            YorickWrapper.AddItem(new MenuItem("NFE", "Packetcasting").SetValue(true));

            YorickWrapper.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat("Frosty " + ChampName + " by newchild");
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (YorickWrapper.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (YorickWrapper.Item("HarrassActive").GetValue<KeyBind>().Active)
            {
                Harrass();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Drawing.DrawCircle(Player.Position, E.Range, Color.AliceBlue);
            Drawing.DrawCircle(Player.Position, E.Range, Color.Black);
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast();

            }
            if (target.IsValidTarget(W.Range) && W.IsReady())
            {
                W.Cast(target, YorickWrapper.Item("NFE").GetValue<bool>());
            }
            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(target, YorickWrapper.Item("NFE").GetValue<bool>());
            }
            if (YorickWrapper.Item("useR").GetValue<Slider>().Value < ((Player.Health/Player.MaxHealth)*100) && R.IsReady())
            {
                R.Cast(Player, YorickWrapper.Item("NFE").GetValue<bool>());
            }
        }
        public static void Harrass()
        {
            var target = SimpleTs.GetTarget(W.Range, SimpleTs.DamageType.Physical);
            if (YorickWrapper.Item("useEH").GetValue<bool>() && E.IsReady() && target.IsValidTarget(E.Range))
            {
                
                E.Cast(target,YorickWrapper.Item("NFE").GetValue<bool>());
            }
            if (YorickWrapper.Item("useWH").GetValue<bool>() && W.IsReady() && target.IsValidTarget(W.Range))
            {

                W.Cast(target, YorickWrapper.Item("NFE").GetValue<bool>());
            }
        }
    }
}
