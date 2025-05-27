using UnityEngine;

public class MagicInteractSkill : ISkill
{
    private GameObject character;
    private MagicInteractSO data;

    public MagicInteractSkill(GameObject character, MagicInteractSO data)
    {
        this.character = character;
        this.data = data;
    }

    public void Activate()
    {
        if (Physics.Raycast(character.transform.position, character.transform.forward, out RaycastHit hit, data.interactRange))
        {
            if (hit.collider.TryGetComponent<IMagicInteractable>(out var interactable))
            {
                interactable.MagicInteract();
            }
        }
    }

    public bool CanActivate() => true;
    public void Deactivate() { }
}
