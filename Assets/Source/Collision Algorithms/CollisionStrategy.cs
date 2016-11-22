using System.Collections.Generic;

public abstract class CollisionStrategy
{
    public abstract void CheckForCollisions(IEnumerable<ZeroHitbox> hitboxList);
}