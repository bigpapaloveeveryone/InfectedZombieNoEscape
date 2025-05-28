using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache 
{
    private static Dictionary<GameObject, Bullet> bullet = new Dictionary<GameObject, Bullet>();

    public static Bullet GetBullet(GameObject obj)
    {
        if (!bullet.ContainsKey(obj))
        {
            Bullet obs = obj.GetComponent<Bullet>();
            if (obs != null)
            {
                bullet.Add(obj, obs);
            }
        }
        return bullet.ContainsKey(obj) ? bullet[obj] : null;
    }

    private static Dictionary<Collider, ZombieHand> hand = new Dictionary<Collider, ZombieHand>();

    public static ZombieHand GetZombieHand(Collider collider)
    {
        if (!hand.ContainsKey(collider))
        {
            hand.Add(collider, collider.GetComponent<ZombieHand>());
        }

        return hand[collider];
    }


    /*    private static Dictionary<GameObject, Obstacle> obstacle = new Dictionary<GameObject, Obstacle>();

        public static Obstacle GetObstacle(GameObject obj)
        {
            if (!bullet.ContainsKey(obj))
            {
                Obstacle obs = obj.GetComponent<Obstacle>();
                if (obs != null)
                {
                    obstacle.Add(obj, obs);
                }
            }
            return obstacle.ContainsKey(obj) ? obstacle[obj] : null;
        }*/
}
