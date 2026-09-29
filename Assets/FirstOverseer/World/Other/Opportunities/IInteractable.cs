namespace FirstOverseer.World.Opportunities
{
    public interface IInteractable
    {
        string InteractionDescriptionKey { get; }
        bool CanInteract { get; }
        void Interact();
    }
}