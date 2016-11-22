using System.Collections.Generic;

public class CollisionSolver
{
    CollisionStrategy strategy;

    public CollisionSolver(CollisionStrategy strategy)
    {
        this.strategy = strategy;
    }

    public void SolveCollisions(IEnumerable<ZeroHitbox> hitboxList)
    {
        strategy.CheckForCollisions(hitboxList);
    }
}