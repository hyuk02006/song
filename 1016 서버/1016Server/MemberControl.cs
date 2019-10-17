using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1016Server
{
    class Member
    {
        public string Id { get; set; }
        public string Pw { get; set; }
        public string Name { get; private set; }
        public int Age { get; set; }

        public Member(string id, string pw, string name, int age)
        {
            Id = id;
            Pw = pw;
            Name = name;
            Age = age;
        }

    }

    class MemberControl
    {
        private List<Member> memlist = new List<Member>();

        #region 싱글톤 패턴
        public static MemberControl Instance { get; private set; }

        static MemberControl()
        {
            Instance = new MemberControl();
        }

        private MemberControl()
        {

        }
        #endregion 

        public bool Insert(Member mem)
        {
            try
            {
                memlist.Add(mem);
                return true;
            }
            catch(Exception )
            {
                return false;
            }
        }

        public bool Delete(string id)
        {
            foreach (Member mem in memlist)
            {
                if(mem.Id.Equals(id))
                {
                    memlist.Remove(mem);
                    return true;
                }
            }
            return false;
        }
        public Member Login(string id,string pw)
        {
            foreach (Member mem in memlist)
            {
                if (mem.Id.Equals(id)&& mem.Pw.Equals(pw))
                {

                    return mem;
                  
                }
            }
            return null;
        }
        public bool Logout(string id)
        {
            foreach (Member mem in memlist)
            {
                if (mem.Id.Equals(id))

                {


                    return true;
                }
            }
            return false;
        }


    }
}
