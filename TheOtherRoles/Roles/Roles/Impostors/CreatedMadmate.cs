using TheOtherRoles.Roles.Core.Bases;

namespace TheOtherRoles.Roles.Impostor;
public sealed class CreatedMadmate : RoleBase
{
    public PlayerControl createdMadmate;

    public bool canEnterVents;
    public bool hasImpostorVision;
    public bool canSabotage;
    public bool canFixComm;
    public bool canDieToSheriff;
    public bool hasTasks;
    public int numTasks;

    public bool tasksComplete(PlayerControl player)
    {
        if (!hasTasks) return false;

        int counter = 0;
        int totalTasks = numTasks;
        if (totalTasks == 0) return true;
        foreach (var task in player.Data.Tasks)
            if (task.Complete)
                counter++;
        return counter >= totalTasks;
    }

    public void clearAndReload()
    {
        createdMadmate = null;
        canEnterVents = CustomOptionHolder.createdMadmateCanEnterVents.getBool();
        canDieToSheriff = CustomOptionHolder.createdMadmateCanDieToSheriff.getBool();
        hasTasks = CustomOptionHolder.createdMadmateAbility.getBool();
        canSabotage = CustomOptionHolder.createdMadmateCanSabotage.getBool();
        canFixComm = CustomOptionHolder.createdMadmateCanFixComm.getBool();
        numTasks = (int)CustomOptionHolder.createdMadmateCommonTasks.getFloat();
    }
}
