using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WorldPhysics
{
    // Мировая главная ось - Z, главная ось ленты Мёбиуса
    // Мировая плоскость Z = 0, режет ленту на две равные части
    // Мировой угол точки - угол поворота проекции радиус-вектора на мировую плоскость вокруг мировой оси отсчитывая от оси X
    static private Vector3 WORLD_AXIS = new Vector3(0f, 0f, 1f);
    static private float GROUND_MARGIN_METRES = 0.5f;
    static private float SQ_HALF_TER_WIDTH = 25.0f; //ширина тропинки == 2 * sqrt(SQ_HALF_TER_WIDTH)
    static private float GROUND_ELASTIC_COEF = -1500;
    static private float GRAVITY_COEF = 5;
    static private float MAX_DRAG_COEF = -12;
    static private float GO_HOME_COEF = 0.004f;
    static private float GO_HOME_POINT_H = 2.0f;
    static private float terrainRadius = 20f;

    static public float GetTerrainRadius()
    {
        return terrainRadius;
    }

    static public Vector3 GetTotalAccel(Vector3 position, Vector3 velocity) // гравитация + реакция опоры ленты
    {
        float depth = GetDepth(position);
        if (IsInOuterSpace(position)) return GoHomeAccel(position);
        //высота зоны действия аэродинамического сопротивления равна половине ширины тропинки:
        float halfTerWidth = Mathf.Sqrt(SQ_HALF_TER_WIDTH);
        //сопротивление максимально при утоплении в грунт для демпфирования колебаний при приземлениях, с высотой убывает по параболе:
        float dragCoef = GetDepth(position) > 0.0 ? MAX_DRAG_COEF : MAX_DRAG_COEF * Mathf.Pow((halfTerWidth - GetUnsignedLocalHeight(position)) / halfTerWidth, 4);
        Vector3 dragAccel = velocity * dragCoef; //аэродинамическое сопротивление, действует только возле ленты        
        if (depth > 0) return GetLocalGravity(position) * (GROUND_ELASTIC_COEF * depth) + dragAccel;
        return GetLocalGravity(position) * GRAVITY_COEF + dragAccel;
    }

    static public float GetDepth(Vector3 position)
    {
        return GROUND_MARGIN_METRES - GetUnsignedLocalHeight(position);
    }
    static public Vector3 GoHomeAccel(Vector3 position)
    {
        Vector3 projOnWorldPlane = position;
        projOnWorldPlane.z = 0f;
        projOnWorldPlane = projOnWorldPlane.normalized;
        // Определяем пересечение местной горизонтальной плоскости "земли" и мировой плоскости (нужна точка, принадлежащая горизонтальной плоскости):
        Vector3 grndOrig = projOnWorldPlane * terrainRadius;
        Vector3 target = grndOrig - GetLocalGravity(position) * GO_HOME_POINT_H;
        return (target - position).normalized * (position - target).magnitude * GO_HOME_COEF;
    }

    static public bool IsInOuterSpace(Vector3 position)
    {
        Vector3 projOnWorldPlane = position;
        projOnWorldPlane.z = 0f;
        projOnWorldPlane = projOnWorldPlane.normalized;
        // Определяем пересечение местной горизонтальной плоскости "земли" и мировой плоскости (нужна точка, принадлежащая горизонтальной плоскости):
        Vector3 grndOrig = projOnWorldPlane * terrainRadius;
        return (position - grndOrig).sqrMagnitude > SQ_HALF_TER_WIDTH;
    }

    static public float GetUnsignedLocalHeight(Vector3 position)
    {
        Vector3 projOnWorldPlane = position;
        projOnWorldPlane.z = 0f;
        projOnWorldPlane = projOnWorldPlane.normalized;
        float worldAngleCos = Vector3.Dot(new Vector3(1f, 0f, 0f), projOnWorldPlane);
        float worldAngle = Mathf.Acos(worldAngleCos);
        if (position.y < 0f) worldAngle = Mathf.PI * 2 - worldAngle;

        // Ось местного вращения ленты, касательная к окружности - сечению ленты мировой плоскостью:
        Vector3 grndRotationAxis = Vector3.Cross(WORLD_AXIS, projOnWorldPlane).normalized;    

        // 90 при переводе в радианы-градусы, т. к. местная крутка террейна равна половине мирового угла, форма ленты:
        Quaternion grndItemRot = Quaternion.AngleAxis(worldAngle * 90f / Mathf.PI, grndRotationAxis);
        Vector3 grndNormal = grndItemRot * projOnWorldPlane;

        // Определяем пересечение местной горизонтальной плоскости "земли" и мировой плоскости (нужна точка, принадлежащая горизонтальной плоскости):
        Vector3 grndOrig = projOnWorldPlane * terrainRadius;
        // Вектор из точки, принадлежащей плоскости, в рассматриваемую точку:
        Vector3 grndToPos = position - grndOrig;
        // Скалярное расстояние от плоскости до рассматриваемой точки, может быть отрицательным, здесь важен именно его знак:
        return Mathf.Abs(Vector3.Dot(grndToPos, grndNormal));  
    }
    static public Vector3 GetLocalGravity(Vector3 position)
    {
        Vector3 projOnWorldPlane = position;
        projOnWorldPlane.z = 0f;
        projOnWorldPlane = projOnWorldPlane.normalized;
        float worldAngleCos = Vector3.Dot(new Vector3(1f, 0f, 0f), projOnWorldPlane);
        float worldAngle = Mathf.Acos(worldAngleCos);
        if (position.y < 0f) worldAngle = Mathf.PI * 2 - worldAngle;

        // Ось местного вращения ленты, касательная к окружности - сечению ленты мировой плоскостью:
        Vector3 grndRotationAxis = Vector3.Cross(WORLD_AXIS, projOnWorldPlane).normalized;

//Ось местного вращения - OY, это некорректно, но это так сделано:
        //Vector3 grndRotationAxis = new Vector3(0, 1, 0); 
        // 90 при переводе в радианы-градусы, т. к. местная крутка террейна равна половине мирового угла, форма ленты:
        Quaternion grndItemRot = Quaternion.AngleAxis(worldAngle * 90f / Mathf.PI, grndRotationAxis);
        Vector3 grndNormal = grndItemRot * projOnWorldPlane;
        // Определяем пересечение местной горизонтальной плоскости "земли" и мировой плоскости (нужна точка, принадлежащая горизонтальной плоскости):
        Vector3 grndOrig = projOnWorldPlane * terrainRadius;
        // Вектор из точки, принадлежащей плоскости, в рассматриваемую точку:
        Vector3 grndToPos = position - grndOrig;
        // Скалярное расстояние от плоскости до рассматриваемой точки, может быть отрицательным, здесь важен именно его знак:
        float posToGrndDist = Vector3.Dot(grndToPos, grndNormal);
        Vector3 gravityDirection = -posToGrndDist * grndNormal;
        return gravityDirection.normalized;
    }

}
