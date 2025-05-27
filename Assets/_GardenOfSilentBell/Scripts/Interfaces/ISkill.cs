public interface ISkill
{
    void Activate();
    bool CanActivate();
    void Deactivate(); // Optional, for passive/buff-based skills
}

