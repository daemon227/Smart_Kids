namespace DACN.Account
{
    public static class AuthLocalService
    {
        public static bool Register(string username, string password, string name)
        {
            // Kiểm tra input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            var dm = LocalDataManager.Instance;

            // Kiểm tra tài khoản đã tồn tại
            foreach (var account in dm.data.userAccounts)
            {
                if (account.username == username && account.acountStatus == true)
                    return false; // Tài khoản đã tồn tại
            }

            // Tạo tài khoản mới
            var newAccount = new UserAccount(username, password, name);
            dm.data.userAccounts.Add(newAccount);
            dm.Save();
            return true;
        }

        public static bool Login(string username, string password)
        {
            var accounts = LocalDataManager.Instance.data.userAccounts;
            if (accounts == null || accounts.Count == 0) 
                return false;

            foreach (var account in accounts)
            {
                if (account.acountStatus == false)
                    continue;
                if (account.username == username && account.password == password)
                    return true;
            }
            return false;
        }

        public static UserAccount GetCurrentUser(string username)
        {
            var accounts = LocalDataManager.Instance.data.userAccounts;
            if (accounts == null) return null;

            foreach (var account in accounts)
            {
                if (account.username == username)
                    return account;
            }
            return null;
        }
    }

}
