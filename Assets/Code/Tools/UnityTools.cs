/* Purpose: This class has a bundle of tool.
 * Notable features:
 *      Can find the closest object with a given tag.
 *      Can find all of the points inside of a Sphere with parameters.
 *      Can find all of the points inside of a Circle with parameters.
 *      Can find all of the points inside of an Ellipsoid with parameters.
 *      Can find all of the points inside of a Cube with parameters.
 *      Can return a ceilinged Vector3 with parameters.
 * 
 * Special Notes: N/A.
 * 
 * Author: Devyn Cyphers; Devcon
 */

using System;
using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static class UnityTools {

    // Finds the closest object with a given tag.
    public static GameObject ClosestWithTag(GameObject target, string tag) {
        GameObject value = null;
        GameObject[] objectsWithTag = GameObject.FindGameObjectsWithTag(tag);
        float closestDist = Mathf.Infinity;

        foreach (GameObject obj in objectsWithTag) {
            float testingDist = Vector3.Distance(target.transform.position, obj.transform.position);
            if (testingDist < closestDist) {
                closestDist = testingDist;
                value = obj;
            }
        }

        return value;
    }

    // Finds all of the points inside of a sphere with parameters.
    // radius(int) of sphere, divisor(int) useful if you only want every 5th or so point using an absolute radius(within this distance), multiplier(int) useful if you only want every 5th or so point using an abstract radius(this many points).
    public static List<Vector3> PointsInsideSphere(int radius, int divisor, int multiplier) {
        List<Vector3> values = new List<Vector3>();

        for (int i = -radius; i <= radius; i++) {
            for (int j = -radius; j <= radius; j++) {
                for (int k = -radius; k <= radius; k++) {
                    if (i * i + j * j + k * k <= radius * radius) {
                        values.Add(new Vector3(j * multiplier, k * multiplier, i * multiplier));
                    }
                }
            }
        }

        return values;
    }

    // Finds all of the points inside of a Circle with parameters.
    // radius(int) of sphere, divisor(int) useful if you only want every 5th or so point using an absolute radius(within this distance), multiplier(int) useful if you only want every 5th or so point using an abstract radius(this many points), zIndex(float) sets the z coordinate of the vector3.
    public static List<Vector3> PointsInsideCircle(int radius, int divisor, int multiplier, float zIndex) {
        List<Vector3> values = new List<Vector3>();

        for (int i = -radius/divisor; i <= radius/divisor; i++) {
            for (int j = -radius/divisor; j <= radius/divisor; j++) {
                if (i * i + j * j <= radius * radius) {
                    values.Add(new Vector3(j * multiplier, i * multiplier, zIndex));
                }
            }
        }

        return values;
    }

    // Given a point it will return true if its inside a circle.
    // point(Vector2), radius(int) of circle.
    public static bool isPointInsideCircle(Vector2 point, int radius) {
        return (point.x + point.y * point.y <= radius * radius) ? true : false;
    }

    // Finds all of the points inside of an ellipsoid with parameters.
    // radii(Vector3) of ellipse, divisor(Vector3) useful if you only want every 5th or so point using am absolute radius(within this distance), multiplier(Vector3) useful if you only want every 5th or so point using an abstract radius(this many points).
    public static List<Vector3> PointsInsideEllipse(Vector3 radii, Vector3 divisor, Vector3 multiplier) {
        List<Vector3> values = new List<Vector3>();

        for (float i = -radii.x / divisor.x; i < radii.x / divisor.x; i++) {
            for (float j = -radii.y / divisor.y; j < radii.y / divisor.y; j++) {
                for (float k = -radii.z / divisor.z; k < radii.z / divisor.z; k++) {
                    if (((i / radii.x) * (i / radii.x) + (j / radii.y) * (j / radii.y) + (k / radii.z) * (k / radii.z)) <= 1) {
                        values.Add(new Vector3(j * multiplier.x, k * multiplier.y, i * multiplier.z));
                    }
                }
            }
        }
        return values;
    }

    // Finds the closest angle out of a list of possible to the given direction.
    public static Vector3 ClosestAngle(Vector3 direction, Vector3[] angles) {
        Vector3 value = new Vector3();
        float closestAngl = Mathf.Infinity;
        foreach (Vector3 vec3 in angles) {
            float testingAngl = Mathf.Abs((vec3 - direction).sqrMagnitude);
            if (testingAngl < closestAngl) {
                closestAngl = testingAngl;
                value = vec3;
            }
        }

        return value;
    }

    // Finds all of the points inside of a cube with parameters.
    // Length(Vector3) of cube, divisor(int) useful if you only want every 5th or so point using am absolute radius(within this distance), multiplier(Vector3) useful if you only want every 5th or so point using an abstract radius(this many points).
    public static List<Vector3> PointsInsideCube(Vector3 length, int divisor, Vector3 multiplier) {
        List<Vector3> values = new List<Vector3>();
        for (float i = -length.x / divisor; i < length.x / divisor; i++) {
            for (float j = -length.y / divisor; j < length.y / divisor; j++) {
                for (float k = -length.z / divisor; k < length.z / divisor; k++) {
                    values.Add(new Vector3(j * multiplier.x, k * multiplier.y, i * multiplier.z));
                }
            }
        }
        return values;
    }

    // Returns a ceilinged Vector3 to a given place value.
    public static Vector3 Ceiling(Vector3 position, int places) {
        Vector3 value = new Vector3();
        float x = position.x, y = position.y, z = position.z;

        if (x > 0) { x = (float)Math.Ceiling(x); } else { x = (float)Math.Floor(x); }
        if (y > 0) { y = (float)Math.Ceiling(y); } else { y = (float)Math.Floor(y); }
        if (z > 0) { z = (float)Math.Ceiling(z); } else { z = (float)Math.Floor(z); }

        x = (float)Math.Round(x / places, 0, MidpointRounding.AwayFromZero) * places;
        y = (float)Math.Round(y / places, 0, MidpointRounding.AwayFromZero) * places;
        z = (float)Math.Round(z / places, 0, MidpointRounding.AwayFromZero) * places;

        value = new Vector3(x, y, z);
        return value;
    }

    // Returns the master GameObject.
    public static GameObject GetMasterController() {
        return GameObject.FindGameObjectWithTag("GameController");
    }
}