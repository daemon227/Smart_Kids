using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DACN.Account
{
    public class AppData
    {
        public List<UserAccount> userAccounts = new List<UserAccount>();
    }
    [Serializable]
    public class UserAccount
    {
        public UserAccount(string username, string password, string name)
        {
            this.username = username;
            this.password = password;
            this.name = name;
            this.role = 0;
            this.acountStatus = true;
            this.childAccounts = new List<ChildAccount>();
        }
        public string username;
        public string password;
        public string name;
        public int role;// 0: parent, 1: admin;
        public bool acountStatus; //true: active, false: banned
        public List<ChildAccount> childAccounts;
    }
    [Serializable]
    public class ChildAccount
    {
        public string childId;
        public string password;
        public string name;
        public int age = 0;
        public int avatarId = 0;
        public bool acountStatus; //true: active, false: banned
        public int score;
        public int dotLevel;
        public int memoryLevel;
        public int quizLevel;
        public bool isLimitedTimeMode = false;
        public int limitedTimePerDay = 60; // in seconds

    }


}

