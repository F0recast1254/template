using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Template 
{

    public class GameWorld
    {
        private static GameWorld _instance = null;

        public static GameWorld Instance 
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new GameWorld();
                }
                return _instance;
            }
        }

        private Room _entrance;
        public Room Entrance {get { return _entrance;  }}
        private Room _exit;


        private Dictionary<ITrigger, IGameEvent> _worldChanges; 


        private GameWorld() 
        {
            _worldChanges = new Dictionary<ITrigger, IGameEvent>();
            _entrance = CreateWorld();
            NotificationCenter.Instance.AddObserver("PlayerWillEnterRoom", PlayerWillEnterRoom);
            NotificationCenter.Instance.AddObserver("PlayerDidEnterRoom", PlayerDidEnterRoom);

        }

        public void PlayerWillEnterRoom (Notification notification)
        {
            Player player = (Player) notification.Object;
            if (player != null)
            {
                //player.WarningMessage("Player will leave " + player.CurrentRoom.Tag);
            }
        }

        public void PlayerDidEnterRoom (Notification notification)
        {
            Player player = (Player) notification.Object;
            if (player != null)
            {
                if (player.CurrentRoom == _exit)
                player.InfoMessage("Player did arrive at the exit");
            }
            IGameEvent wc = null;
            _worldChanges.TryGetValue(player.CurrentRoom, out wc);

            if (wc != null)
            {
                wc.Execute(player);
            }
        }

        public Room CreateWorld()
        {
            Room outside = new Room("outside the main entrance of the university");
            Room scctparking = new Room("in the parking lot at SCCT");
            Room boulevard = new Room("on the boulevard");
            Room universityParking = new Room("in the parking lot at University Hall");
            Room parkingDeck = new Room("in the parking deck");
            Room scct = new Room("in the SCCT building");
            Room theGreen = new Room("in the green in from of Schuster Center");
            Room universityHall = new Room("in University Hall");
            Room schuster = new Room("in the Schuster Center");
            Room davidson = new Room("in the Davidson Center");
            Room clockTower = new Room("in the Clock Tower");
            Room greekCenter = new Room("in the Greek Center");
            Room woodall = new Room("in Woodall");


            Door door = Door.Connect(outside, boulevard, "west", "east");


            door = Door.Connect(boulevard, scctparking, "south", "north");

     
            door = Door.Connect(boulevard, theGreen, "west", "east");

  
            door = Door.Connect(boulevard, universityParking, "north", "south");

      
            door = Door.Connect(scctparking, scct, "west", "east");


            door = Door.Connect(scct, schuster, "north", "south");

  
            door = Door.Connect(schuster, universityHall, "north", "south");
            
            /*
            RegularLock rl = new RegularLock();
            
            Keyed keyed = new Keyed("key1");
            rl.Keyed = keyed;
            */

            ILockable rl =LockableFacade.MakeLockable("Regular", "key1");
            door.Lockable = rl;
            door.Close();
            door.Lock();
            IItem key = door.Remove();
            clockTower.Drop(key);

            door = Door.Connect(schuster, theGreen, "east", "west");
            

            
          
            door = Door.Connect(universityParking, universityHall, "east", "west");

         
            door = Door.Connect(parkingDeck, universityParking, "south", "north");

            // inaccessible part of the world
       
            door = Door.Connect(davidson, clockTower, "west", "east");

          
            door = Door.Connect(clockTower, greekCenter, "north", "south");

          
            door = Door.Connect(clockTower, woodall, "south", "north");

            // info to connect the parts of the world, Schuster will gain an exit (4th exit)
            // "unlocking" another part of the world
            // idea being once you enter the schuster center the Davidson is unlocked via Schuster's 
            // new west exit
            WorldChange wc = new WorldChange(universityHall, schuster, davidson, "west", "east");
            _worldChanges[universityHall] = wc;

            IRoomDelegate tr = new TrapRoom("shazam!");
            scct.RoomDelegate = tr;
           // tr.ContainingRoom = scct;

            parkingDeck.RoomDelegate = tr;
            //tr.ContainingRoom = parkingDeck;
            IRoomDelegate er = new EchoRoom(3);
            greekCenter.RoomDelegate = er;
            IItem newitem = new Item("new iPad", 0.75f);
            parkingDeck.Drop(newitem);
            IItem decorator = new Item("cover", 0.25f);
            newitem.AddDecorator(decorator);
            _exit = parkingDeck;
            decorator = new Item("pen", 0.1f);
            newitem.AddDecorator(decorator);

            return outside;
        }
    }


}