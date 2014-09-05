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
namespace Frosty_Renekton
{
    class Program
    {
        public static HitChance test;
        public static string ChampName = "Renekton";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Spell Q, W, E, R, AA;

        public static Menu RenektonWrapper;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;
            Q = new Spell(SpellSlot.Q, 225);
            W = new Spell(SpellSlot.W, Player.AttackRange);
            E = new Spell(SpellSlot.E, 450);
            R = new Spell(SpellSlot.R, float.MaxValue);
            W.SetSkillshot(0.5f, 50.0f, 20.0f, false, SkillshotType.SkillshotLine);

            RenektonWrapper = new Menu(ChampName, ChampName, true);

            RenektonWrapper.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(RenektonWrapper.SubMenu("Orbwalker"));
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            RenektonWrapper.AddSubMenu(ts);
            RenektonWrapper.AddSubMenu(new Menu("Combo", "Combo"));
            RenektonWrapper.SubMenu("Combo").AddItem(new MenuItem("useR", "Use R Life?").SetValue(new Slider(40, 0, 100)));
            RenektonWrapper.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "COMBO!").SetValue(new KeyBind(32, KeyBindType.Press)));
            RenektonWrapper.SubMenu("Combo").AddItem(new MenuItem("useRc", "Minimum R Hit").SetValue(new Slider(2, 1, 5)));
            RenektonWrapper.SubMenu("Combo").AddItem(new MenuItem("chance", "Hitchance of R").SetValue(new Slider(2, 1, 4)));
            RenektonWrapper.AddItem(new MenuItem("NFE", "Packet Casting").SetValue(true));
            RenektonWrapper.AddSubMenu(new Menu("Interrupt", "Interrupt"));
            RenektonWrapper.AddSubMenu(new Menu("Harrass", "Harrass"));
            RenektonWrapper.AddSubMenu(new Menu("Farm", "Farm"));
            RenektonWrapper.SubMenu("Harrass").AddItem(new MenuItem("HarrassActive", "Harrass").SetValue(new KeyBind("X".ToCharArray()[0], KeyBindType.Press)));
            RenektonWrapper.SubMenu("Farm").AddItem(new MenuItem("FarmActive", "Farm").SetValue(new KeyBind("V".ToCharArray()[0], KeyBindType.Press)));
            RenektonWrapper.SubMenu("Farm").AddItem(new MenuItem("useqf", "Use Q to Farm").SetValue(true));
            RenektonWrapper.SubMenu("Interrupt").AddItem(new MenuItem("usew", "Use W to Interrupt").SetValue(true));
            RenektonWrapper.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat("Frosty " + ChampName + " by newchild");
            Interrupter.OnPosibleToInterrupt += OnPosibleToInterrupt;
        }

        static void Game_OnGameUpdate(EventArgs args)
        {
            if (RenektonWrapper.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (RenektonWrapper.Item("FarmActive").GetValue<KeyBind>().Active)
            {
                Farm();
            }
            if (RenektonWrapper.Item("HarrassActive").GetValue<KeyBind>().Active)
            {
                Harrass();
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            Utility.DrawCircle(Player.Position, Q.Range, Color.Azure);
            Utility.DrawCircle(Player.Position, W.Range, Color.Black);
            Utility.DrawCircle(Player.Position, E.Range, Color.Crimson);
            Utility.DrawCircle(Player.Position, R.Range, Color.Gold);
        }

        public static void Harrass()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(target, RenektonWrapper.Item("NFE").GetValue<bool>());
                while(Player.IsDashing()){

                }
                Q.Cast();
                W.Cast();
                
            }
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;

            if (target.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(target, RenektonWrapper.Item("NFE").GetValue<bool>());

            }
            if (target.IsValidTarget(Player.AttackRange) && W.IsReady())
            {
                W.Cast();
            }
            if (target.IsValidTarget(R.Range) && R.IsReady() && RenektonWrapper.Item("useR").GetValue<Slider>().Value >= ((Player.Health/Player.MaxHealth)*100))
            {

                R.Cast();
            }
            if (target.IsValidTarget(E.Range) && E.IsReady())
            {
                E.Cast(target,RenektonWrapper.Item("NFE").GetValue<bool>());
                E.Cast(target, RenektonWrapper.Item("NFE").GetValue<bool>());
            }
        }
        private static void OnPosibleToInterrupt(Obj_AI_Base target, InterruptableSpell spell)
        {
            if (RenektonWrapper.Item("usew").GetValue<bool>())
            {
                if (Player.Distance(target) < W.Range && W.IsReady())
                {
                    Q.Cast(target, RenektonWrapper.Item("NFE").GetValue<bool>());
                }
            }
            
        }
        public static void Farm()
        {
            if (RenektonWrapper.Item("useqf").GetValue<bool>())
            {
                if (!Orbwalking.CanMove(40)) return;
                var Rangeminions = MinionManager.GetMinions(Player.ServerPosition, Q.Range);
                if (Q.IsReady())
                {
                    foreach (var minion in Rangeminions)
                    {
                        if (minion.IsValidTarget() && HealthPrediction.GetHealthPrediction(minion, (int)Player.Distance(minion) * 1000 / 14000) < 0.75 * DamageLib.getDmg(minion, DamageLib.SpellType.Q))
                        {
                            if (Vector3.Distance(minion.ServerPosition, Player.ServerPosition) > Orbwalking.GetRealAutoAttackRange(Player))
                            {
                                Q.Cast();
                            }
                        }
                            ;
                    }
                        ;
                }
            }
        }
    }
}

