using System;
using System.Threading;
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
        public static int PassiveActive = 0;
        public static int PassiveDmg = 1;
        public static string ChampName = "Twitch";
        public static Orbwalking.Orbwalker Orbwalker;
        public static Obj_AI_Base Player = ObjectManager.Player;
        public static Spell Q, W, E, R;
        public static int Ecost = 50;
        public static Menu TwitchWrapper;
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
        }

        static void Game_OnGameLoad(EventArgs args)
        {
            if (Player.BaseSkinName != ChampName) return;
            Game.PrintChat("Freaky " + ChampName + " by newchild");
            Q = new Spell(SpellSlot.Q, 0);
            W = new Spell(SpellSlot.W, 950);
            E = new Spell(SpellSlot.E, 1200);
            R = new Spell(SpellSlot.R, 850);
            
            W.SetSkillshot(0.25f, 120f, 1400f, true, SkillshotType.SkillshotCircle);
            TwitchWrapper = new Menu(ChampName, ChampName, true);
            TwitchWrapper.AddSubMenu(new Menu("Orbwalker", "Orbwalker"));
            Orbwalker = new Orbwalking.Orbwalker(TwitchWrapper.SubMenu("Orbwalker"));
            var ts = new Menu("Target Selector", "Target Selector");
            SimpleTs.AddToMenu(ts);
            TwitchWrapper.AddSubMenu(ts);
            TwitchWrapper.AddSubMenu(new Menu("Combo", "Combo"));
            TwitchWrapper.SubMenu("Combo").AddItem(new MenuItem("ComboActive", "Combo!").SetValue(new KeyBind(32, KeyBindType.Press)));
            TwitchWrapper.AddSubMenu(new Menu("Draw", "Drawing"));
            TwitchWrapper.SubMenu("Draw").AddItem(new MenuItem("DrawActive", "Enable drawings").SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Toggle)));
            TwitchWrapper.SubMenu("Combo").AddItem(new MenuItem("UseR", "Use R in COmbo").SetValue(new KeyBind("Y".ToCharArray()[0], KeyBindType.Toggle)));
            TwitchWrapper.AddItem(new MenuItem("NFE", "Packetcasting").SetValue(true));
            TwitchWrapper.AddToMainMenu();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnGameUpdate += Game_OnGameUpdate;

            Game.PrintChat("Freaky " + ChampName + " by newchild");
            Game.PrintChat("This is an enhanced version of the marksman plugin!");
        }

        /*static void updatePassive(){
            if(Player.Level == 1){
                PassiveDmg = 1;
            }
            if(Player.Level == 4){
                PassiveDmg = 2;
            }
            if(Player.Level == 7){
                PassiveDmg = 3;
            }
            if(Player.Level == 10){
                PassiveDmg = 4;
            }
            if(Player.Level == 13){
                PassiveDmg = 5;
            }
            if(Player.Level == 16){
                PassiveDmg = 6;
            }
            */
        
        static void Game_OnGameUpdate(EventArgs args)
        {
            updatePassive();
            if (E.Level > 0)
            {
                Ecost = (E.Level * 10) + 40;
            }

            if (TwitchWrapper.Item("ComboActive").GetValue<KeyBind>().Active)
            {
                Combo();
            }

        }
        /*
        static void disablePassive(object o){
            PassiveActive -= 1;
        }
        static void Orbwalking_BeforeAttack(){
           PassiveActive = 6;
           threadedFunc();
            
        }
        static void threadedFunc(){
            while(PassiveActive > 0){Timer t = new Timer(disablePassive, null, 6, 1000)}
        }
        */
        static void Drawing_OnDraw(EventArgs args)
        {
            if (TwitchWrapper.Item("DrawActive").GetValue<KeyBind>().Active)
            {


                Utility.DrawCircle(Player.Position, W.Range, Color.Black, 15);
                Utility.DrawCircle(Player.Position, E.Range, Color.Crimson, 10);
                Utility.DrawCircle(Player.Position, Player.AttackRange, Color.Brown);
            }
        }

        public static void Combo()
        {
            var target = SimpleTs.GetTarget(E.Range, SimpleTs.DamageType.Physical);
            if (target == null) return;
            if (target.Team != Player.Team)
            {
                if (target.IsValidTarget(W.Range) && W.IsReady() && (Player.Mana > (50+Ecost)))
                {

                    W.Cast(target, TwitchWrapper.Item("NFE").GetValue<bool>());
                }
                if (target.IsValidTarget(E.Range) && E.IsReady())
                {
                    int ExpungeStacks = (from buff in target.Buffs
                    where buff.DisplayName.ToLower() == "twitchdeadlyvenom"
                    select buff.Count).FirstOrDefault();
                    
                    //int DoT = ExpungeStacks*PassiveDmg*PassiveActive;
                    
                    //Game.PrintChat("Expected DoT= " + (string) DoT);
                    
                    foreach (
                    var hero in
                    ObjectManager.Get<Obj_AI_Hero>()
                        .Where(
                            hero =>
                                hero.IsValidTarget(E.Range) &&
                                (E.GetDamage(hero)) - 15 > hero.Health))
                        E.Cast();
                }
                if (target.IsValidTarget((R.Range - 50)) && R.IsReady())
                {
                    if (TwitchWrapper.Item("UseR").GetValue<KeyBind>().Active)
                    {
                        R.Cast();
                    }
                    
                }
            }
        }

    }
}

