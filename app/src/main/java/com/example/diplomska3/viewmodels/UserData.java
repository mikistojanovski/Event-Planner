package com.example.diplomska3.viewmodels;

public class UserData {

        private String Username;
        private String Password;
        private String Email;
        private int  IfHost;

        public UserData() {
        }
        public UserData(String Username,String Password,String Email,int IfHost){
            this.Username=Username;
	    this.Password=Password;
	    this.Email=Email;
            this.IfHost=IfHost;
        }

        public String getUsername() {
            if(Username!=null){
                return Username;
            }
            else {
                return "miki";
            }
        }

        public void setUsername(String contact) {
            Username = contact;
        }

        public int getIfHost() {
            return IfHost;
        }
         public String getEmail() {
        return Email;
    }
         public String getPassword() {
        return Password;
    }

        public void setIfHost(int address) {
            this.IfHost = address;
        }
 }

