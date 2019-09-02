using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public static class Extensions2D
{

    public static void RotateTO(this Transform obj, Transform target)
    {
        var dir = target.position - obj.position;
        dir = dir.normalized;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public static void RotateTO(this Transform obj, Vector2 dir)
    {
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    public static void RotateTO(this Transform obj, Vector2 dir, float speed)
    {
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        obj.rotation = Quaternion.RotateTowards(obj.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), speed);
    }

    public static bool LineIntersection(Vector2 line1point1, Vector2 line1point2, Vector2 line2point1, Vector2 line2point2) {
 
        Vector2 a = line1point2 - line1point1;
        Vector2 b = line2point1 - line2point2;
        Vector2 c = line1point1 - line2point1;
 
        float alphaNumerator = b.y * c.x - b.x * c.y;
        float betaNumerator  = a.x * c.y - a.y * c.x;
        float denominator    = a.y * b.x - a.x * b.y;
 
        if (denominator == 0) {
            return false;
        } else if (denominator > 0) {
            if (alphaNumerator < 0 || alphaNumerator > denominator || betaNumerator < 0 || betaNumerator > denominator) {
                return false;
            }
        } else if (alphaNumerator > 0 || alphaNumerator < denominator || betaNumerator > 0 || betaNumerator < denominator) {
            return false;
        }
        return true;
    }
    
    public static Vector2 PerpendicularClockwise(this Vector2 vector2)
    {
        return new Vector2(vector2.y, -vector2.x);
    }

    public static Vector2 PerpendicularCounterClockwise(this Vector2 vector2)
    {
        return new Vector2(-vector2.x, vector2.y);
    }
    
    public static Bounds GetMaxBounds(this GameObject g) {
        var b = new Bounds(g.transform.position, Vector3.zero);

        foreach (Renderer r in g.GetComponentsInChildren<Renderer>())
        {
            if(r.GetComponent<ParticleSystem>())
                continue;
            
            b.Encapsulate(r.bounds);
        }

        return b;
    }
}
