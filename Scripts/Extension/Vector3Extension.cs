using System;
using System.Collections.Generic;
using UnityEngine;

namespace NamPhuThuy.Common
{
    public static class Vector3Extension
    {
        
        #region Random Point Spawning

        /// <summary>
        /// Get random point in sphere around center
        /// </summary>
        public static Vector3 GetRandomPointInSphere(this Vector3 center, float radius)
        {
            return center + UnityEngine.Random.insideUnitSphere * radius;
        }

        /// <summary>
        /// Get random point on sphere surface around center
        /// </summary>
        public static Vector3 GetRandomPointOnSphere(this Vector3 center, float radius)
        {
            return center + UnityEngine.Random.onUnitSphere * radius;
        }

        /// <summary>
        /// Get random point in circle (2D, XY plane) around center
        /// </summary>
        /// <param name="center">Center position</param>
        /// <param name="radius">Circle radius</param>
        /// <param name="xBias">Bias on X axis: -1 (left) to 1 (right), 0 = no bias</param>
        /// <param name="yBias">Bias on Y axis: -1 (down) to 1 (up), 0 = no bias</param>
        public static Vector3 GetRandomPointInCircle(this Vector3 center, float radius, float xBias = 0f, float yBias = 0f)
        {
            // Generate random point
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle;
            
            // Apply bias using power function for smooth distribution
            // Bias > 0: shift right/up, Bias < 0: shift left/down
            if (Mathf.Abs(xBias) > 0.01f)
            {
                float sign = Mathf.Sign(randomPoint.x);
                float power = 1f - xBias; // bias=1 -> power=0 (extreme), bias=0 -> power=1 (normal)
                randomPoint.x = sign * Mathf.Pow(Mathf.Abs(randomPoint.x), Mathf.Max(0.1f, power));
            }
            
            if (Mathf.Abs(yBias) > 0.01f)
            {
                float sign = Mathf.Sign(randomPoint.y);
                float power = 1f - yBias;
                randomPoint.y = sign * Mathf.Pow(Mathf.Abs(randomPoint.y), Mathf.Max(0.1f, power));
            }
            
            randomPoint *= radius;
            return new Vector3(center.x + randomPoint.x, center.y + randomPoint.y, center.z);
        }

        /// <summary>
        /// Get random point on circle (2D, XY plane) around center
        /// </summary>
        public static Vector3 GetRandomPointOnCircle(this Vector3 center, float radius)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle.normalized * radius;
            return new Vector3(center.x + randomPoint.x, center.y + randomPoint.y, center.z);
        }

        /// <summary>
        /// Get random point in horizontal circle (XZ plane) around center - for ground spawning
        /// </summary>
        public static Vector3 GetRandomPointInHorizontalCircle(this Vector3 center, float radius)
        {
            Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * radius;
            return new Vector3(center.x + randomPoint.x, center.y, center.z + randomPoint.y);
        }

        /// <summary>
        /// Get random point on horizontal circle (XZ plane) around center
        /// </summary>
        public static Vector3 GetRandomPointOnHorizontalCircle(this Vector3 center, float radius)
        {
            float angle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float x = center.x + radius * Mathf.Cos(angle);
            float z = center.z + radius * Mathf.Sin(angle);
            return new Vector3(x, center.y, z);
        }

        /// <summary>
        /// Get random point in annulus (ring) - between minRadius and maxRadius
        /// </summary>
        public static Vector3 GetRandomPointInAnnulus(this Vector3 center, float minRadius, float maxRadius)
        {
            Vector2 randomDirection = UnityEngine.Random.insideUnitCircle.normalized;
            float randomRadius = UnityEngine.Random.Range(minRadius, maxRadius);
            return new Vector3(
                center.x + randomDirection.x * randomRadius,
                center.y,
                center.z + randomDirection.y * randomRadius
            );
        }

        /// <summary>
        /// Get random point in box/cube around center
        /// </summary>
        public static Vector3 GetRandomPointInBox(this Vector3 center, Vector3 size)
        {
            float x = UnityEngine.Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float y = UnityEngine.Random.Range(center.y - size.y / 2, center.y + size.y / 2);
            float z = UnityEngine.Random.Range(center.z - size.z / 2, center.z + size.z / 2);
            return new Vector3(x, y, z);
        }

        /// <summary>
        /// Get random point in box with uniform size
        /// </summary>
        public static Vector3 GetRandomPointInBox(this Vector3 center, float size)
        {
            return GetRandomPointInBox(center, Vector3.one * size);
        }

        /// <summary>
        /// Get multiple random points in sphere
        /// </summary>
        public static List<Vector3> GetRandomPointsInSphere(this Vector3 center, float radius, int count)
        {
            List<Vector3> points = new List<Vector3>(count);
            for (int i = 0; i < count; i++)
            {
                points.Add(GetRandomPointInSphere(center, radius));
            }
            return points;
        }

        /// <summary>
        /// Get multiple random points in circle with minimum distance between them
        /// </summary>
        public static List<Vector3> GetRandomPointsInCircleWithMinDistance(this Vector3 center, float radius, int count, float minDistance)
        {
            List<Vector3> points = new List<Vector3>();
            int maxAttempts = count * 30; // Prevent infinite loop
            int attempts = 0;

            while (points.Count < count && attempts < maxAttempts)
            {
                Vector3 candidate = GetRandomPointInHorizontalCircle(center, radius);
                
                bool isValid = true;
                foreach (Vector3 point in points)
                {
                    if (Vector3.Distance(candidate, point) < minDistance)
                    {
                        isValid = false;
                        break;
                    }
                }

                if (isValid)
                {
                    points.Add(candidate);
                }

                attempts++;
            }

            return points;
        }

        /// <summary>
        /// Get random point within cone direction
        /// </summary>
        public static Vector3 GetRandomPointInCone(this Vector3 origin, Vector3 direction, float distance, float angle)
        {
            Vector3 randomDir = Quaternion.AngleAxis(
                UnityEngine.Random.Range(-angle, angle),
                Vector3.up
            ) * direction;

            float randomDistance = UnityEngine.Random.Range(0f, distance);
            return origin + randomDir.normalized * randomDistance;
        }

        /// <summary>
        /// Get random point on navmesh near center (requires NavMesh)
        /// </summary>
        public static Vector3 GetRandomPointOnNavMesh(this Vector3 center, float radius)
        {
            #if UNITY_AI
            Vector3 randomPoint = GetRandomPointInHorizontalCircle(center, radius);
            UnityEngine.AI.NavMeshHit hit;
            
            if (UnityEngine.AI.NavMesh.SamplePosition(randomPoint, out hit, radius, UnityEngine.AI.NavMesh.AllAreas))
            {
                return hit.position;
            }
            #endif
            
            return GetRandomPointInHorizontalCircle(center, radius);
        }

        /// <summary>
        /// Get evenly distributed random points in circle (Poisson disk sampling)
        /// </summary>
        public static List<Vector3> GetPoissonDiskPoints(this Vector3 center, float radius, float minDistance, int samplesBeforeRejection = 30)
        {
            List<Vector3> points = new List<Vector3>();
            List<Vector3> activeList = new List<Vector3>();

            // Start with center point
            Vector3 firstPoint = center;
            points.Add(firstPoint);
            activeList.Add(firstPoint);

            while (activeList.Count > 0)
            {
                int randomIndex = UnityEngine.Random.Range(0, activeList.Count);
                Vector3 currentPoint = activeList[randomIndex];
                bool foundValidPoint = false;

                for (int i = 0; i < samplesBeforeRejection; i++)
                {
                    // Generate random point in annulus
                    float randomAngle = UnityEngine.Random.Range(0f, 360f) * Mathf.Deg2Rad;
                    float randomRadius = UnityEngine.Random.Range(minDistance, minDistance * 2);
                    
                    Vector3 candidate = new Vector3(
                        currentPoint.x + randomRadius * Mathf.Cos(randomAngle),
                        currentPoint.y,
                        currentPoint.z + randomRadius * Mathf.Sin(randomAngle)
                    );

                    // Check if within radius
                    if (Vector3.Distance(center, candidate) > radius)
                        continue;

                    // Check minimum distance from all existing points
                    bool isValid = true;
                    foreach (Vector3 point in points)
                    {
                        if (Vector3.Distance(candidate, point) < minDistance)
                        {
                            isValid = false;
                            break;
                        }
                    }

                    if (isValid)
                    {
                        points.Add(candidate);
                        activeList.Add(candidate);
                        foundValidPoint = true;
                        break;
                    }
                }

                if (!foundValidPoint)
                {
                    activeList.RemoveAt(randomIndex);
                }
            }

            return points;
        }

        #endregion
        public static Vector3 GetRandomInRange(this Vector3 pos, float rangeX, float rangeY, float rangeZ)
        {
            float randX = UnityEngine.Random.Range(pos.x - rangeX, pos.x + rangeX);
            float randY = UnityEngine.Random.Range(pos.y - rangeY, pos.y + rangeY);
            float randZ = UnityEngine.Random.Range(pos.z - rangeZ, pos.z + rangeZ);
            return new Vector3(randX, randY, randZ);
        }

        public static Vector3 GetRandomInRange(this Vector3 pos, float range)
        {
            float randX = UnityEngine.Random.Range(pos.x - range, pos.x + range);
            float randY = UnityEngine.Random.Range(pos.y - range, pos.y + range);
            float randZ = UnityEngine.Random.Range(pos.y - range, pos.y + range);
            return new Vector3(randX, randY, randZ);
        }

        public static Vector3 Round(this Vector3 vector3, int decimalPlaces = 2)
        {
            return new Vector3(
                (float)Math.Round(vector3.x, decimalPlaces),
               (float)Math.Round(vector3.y, decimalPlaces),
               (float)Math.Round(vector3.z, decimalPlaces));
        }

        public static Vector3 GetProjectionPoint(this Vector3 point, Vector3 lineVect, Vector3 linePoint)
        {
            float t = 0;
            if (lineVect.sqrMagnitude > 0.0001f)
            {
                t = -((linePoint.x - point.x) * lineVect.x
                      + (linePoint.y - point.y) * lineVect.y
                      + (linePoint.z - point.z) * lineVect.z)
                        / lineVect.sqrMagnitude; ;
            }
            Vector3 result = new Vector3(
                linePoint.x + t * lineVect.x,
                linePoint.y + t * lineVect.y,
                linePoint.z + t * lineVect.z);
            return result;
        }

        public static Vector3 GetMiddlePoint(Vector3 pointA, Vector3 pointB)
        {
            return (pointA + pointB) / 2;
        }

        public static List<Vector3> GetConeDirections(this Vector3 direction, float angle, int numOfRay)
        {
            List<Vector3> vects = new List<Vector3>();
            var vector = Quaternion.AngleAxis(-angle, Vector3.up) * direction;
            vects.Add(vector);
            var vector2 = Quaternion.AngleAxis(angle, Vector3.up) * direction;
            vects.Add(vector2);
            float anglePerStep = angle * 2 / numOfRay;
            for (int i = 1; i <= numOfRay; i++)
            {
                var dir = Quaternion.AngleAxis(-angle + anglePerStep * i, Vector3.up) * direction;
                vects.Add(dir);
            }
            return vects;
        }

        public static bool IsInRange(this Vector3 position, Vector3 target, float range)
        {
            return Vector3.SqrMagnitude(position - target) <= range * range;
        }

        public static float TotalDistance(this IEnumerable<Vector3> list)
        {
            float distance = 0;
            bool getLast = false;
            Vector3 last = Vector3.zero;
            foreach (var item in list)
            {
                if (getLast)
                {
                    distance += Vector3.Distance(last, item);
                }
                last = item;
                getLast = true;
            }
            return distance;
        }

        public static List<Vector3> Split(this List<Vector3> list, float distance)
        {
            distance = Mathf.Max(distance, 0.015f);
            int pointCount = Mathf.RoundToInt(list.TotalDistance() / distance);
            return list.Split(pointCount);
        }

        public static List<Vector3> Split(this List<Vector3> list, int pointCount)
        {
            pointCount = Mathf.Max(list.Count, pointCount);
            float distance = list.TotalDistance() / (pointCount - 1);
            List<Vector3> result = new List<Vector3>();
            result.Add(list.FirstElement());
            float tempDistance = 0;
            if (list.Count == 0 || distance < 0.01f) return list;
            //
            for (int i = 0; i < list.Count - 1; i++)
            {
                Vector3 temp = list[i];
                while (true)
                {
                    temp = Vector3.MoveTowards(temp, list[i + 1], distance - tempDistance);
                    tempDistance = 0;
                    result.Add(new Vector3(temp.x, temp.y, temp.z));
                    float distanceBetween = Vector3.Distance(temp, list[i + 1]);
                    if (distanceBetween < distance)
                    {
                        tempDistance = distanceBetween;
                        break;
                    }
                }
            }
            result.Add(list.LastElement());
            return result;
        }

        public static float GetRightZAngle(this Vector3 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            return angle;
        }

        public static List<Vector3> ToPoints(this EdgeCollider2D edge)
        {
            List<Vector3> result = new List<Vector3>();
            foreach (var item in edge.points)
            {
                result.Add(item);
            }
            return result;
        }

        public static bool IsInRect(this Vector3 point, Vector3 rectPosition, Vector3 rectSize)
        {
            return Mathf.Abs(point.x - rectPosition.x) < rectSize.x
               && Mathf.Abs(point.y - rectPosition.y) < rectSize.y
               && Mathf.Abs(point.z - rectPosition.z) < rectSize.z;
        }

        public static Vector3 ClosestPointOnLineSegment(float px, float py, float pz, float ax, float ay, float az, float bx, float by, float bz)
        {
            float apx = px - ax;
            float apy = py - ay;
            float apz = pz - az;
            float abx = bx - ax;
            float aby = by - ay;
            float abz = bz - az;
            float abMag = abx * abx + aby * aby + abz * abz; // Sqr magnitude.
            if (abMag < Mathf.Epsilon) return new Vector3(ax, ay, az);
            // Normalize.
            abMag = Mathf.Sqrt(abMag);
            abx /= abMag;
            aby /= abMag;
            abz /= abMag;
            float mu = abx * apx + aby * apy + abz * apz; // Dot.
            if (mu < 0) return new Vector3(ax, ay, az);
            if (mu > abMag) return new Vector3(bx, by, bz);
            return new Vector3(ax + abx * mu, ay + aby * mu, az + abz * mu);
        }
        /// <summary>
        /// Closest point on a line segment from a given point in 3D.
        /// </summary>
        public static Vector3 ClosestPointOnLineSegment(Vector3 p, Vector3 a, Vector3 b)
        {
            return ClosestPointOnLineSegment(p.x, p.y, p.z, a.x, a.y, a.z, b.x, b.y, b.z);
        }

        #region Modify Coordinates

        public static Vector3 ChangeX(this Vector3 currentValue, float value)
        {
            return new Vector3(value, currentValue.y, currentValue.z);
        }

        public static Vector3 ChangeY(this Vector3 currentValue, float value)
        {
            return new Vector3(currentValue.x, value, currentValue.z);
        }

        public static Vector3 ChangeZ(this Vector3 currentValue, float value)
        {
            return new Vector3(currentValue.x, currentValue.y, value);
        }
        
        public static Vector3 AddZ(this Vector3 currentValue, float value)
        {
            return new Vector3(currentValue.x, currentValue.y, currentValue.z + value);
        }

        #endregion
    }
}
