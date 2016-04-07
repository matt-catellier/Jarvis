using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jarvis_Phase3.Models
{
    public class EditableUserRepo
    {
        private JarvisEntities db = new JarvisEntities();

        public EditableUser getUser(string userID)
        {
            EditableUser myUser = (from u in db.AspNetUsers
                                   where u.Id == userID
                                   select new EditableUser()
                                   {
                                       ID = u.Id,
                                       UserName = u.UserName,
                                       Email = u.Email,
                                       FirstName = u.Account.firstName,
                                       LastName = u.Account.lastName
                                   }).FirstOrDefault();
            return myUser;
        }

        public IEnumerable<EditableUser> getUsers()
        {
            IEnumerable<EditableUser> myUsers = (from u in db.AspNetUsers
                                                 select new EditableUser()
                                                 {
                                                     ID = u.Id,
                                                     UserName = u.UserName,
                                                     Email = u.Email,
                                                     FirstName = u.Account.firstName,
                                                     LastName = u.Account.lastName
                                                 });

            return myUsers;
        }

        public void updateUser(EditableUser editedUser)
        {
            AspNetUser myUpdateUser = db.AspNetUsers.Where(u => u.Id == editedUser.ID)
                                            .FirstOrDefault();
            Account myUpdateAccount = db.Accounts.Where(a => a.accountID == editedUser.ID)
                                            .FirstOrDefault();

            myUpdateUser.UserName = editedUser.UserName;
            myUpdateUser.Email = editedUser.Email;
            myUpdateAccount.firstName = editedUser.FirstName;
            myUpdateAccount.lastName = editedUser.LastName;

            db.SaveChanges();
        }
    }
}