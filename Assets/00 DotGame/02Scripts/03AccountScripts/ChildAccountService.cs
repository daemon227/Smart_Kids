using System.Linq;
using DACN.Account;
namespace DACN.Account
{
public static class ChildAccountService
{
    // ===============================
    // THÊM TÀI KHOẢN CON
    // ===============================
    public static bool AddChild(
        UserAccount parent,
        string username,
        string password,
        string name)
    {
        if (parent == null) return false;

        // Check trùng username
        if (parent.childAccounts.Any(c => c.username == username))
            return false;

        ChildAccount child = new ChildAccount
        {
            username = username,
            password = password,
            name = name,
            acountStatus = true,
            score = 0,
            dotLevel = 1,
            memoryLevel = 1,
            quizLevel = 1
        };

        parent.childAccounts.Add(child);
        LocalDataManager.Instance.Save();
        return true;
    }

    // ===============================
    // SỬA TÀI KHOẢN CON
    // ===============================
    public static bool EditChild(
        UserAccount parent,
        string childUsername,
        string newPassword,
        string newName)
    {
        if (parent == null) return false;

        ChildAccount child = parent.childAccounts
            .FirstOrDefault(c => c.username == childUsername);

        if (child == null) return false;

        child.password = newPassword;
        child.name = newName;

        LocalDataManager.Instance.Save();
        return true;
    }

    // ===============================
    // XÓA TÀI KHOẢN CON
    // ===============================
    public static bool DeleteChild(
        UserAccount parent,
        string childUsername)
    {
        if (parent == null) return false;

        ChildAccount child = parent.childAccounts
            .FirstOrDefault(c => c.username == childUsername);

        if (child == null) return false;

        child.acountStatus = false; // Đánh dấu là bị xóa
        LocalDataManager.Instance.Save();
        return true;
    }

    // ===============================
    // CÀI ĐẶT THỜI GIAN GIỚI HẠN
    // ===============================
    public static bool SetLimitedTime(
        UserAccount parent,
        string childUsername,
        bool isLimited,
        int limitedTimePerDay)
    {
        if (parent == null) return false;

        ChildAccount child = parent.childAccounts
            .FirstOrDefault(c => c.username == childUsername);

        if (child == null) return false;

        child.isLimitedTimeMode = isLimited;
        child.limitedTimePerDay = limitedTimePerDay;

        LocalDataManager.Instance.Save();
        return true;
    }
}
}
