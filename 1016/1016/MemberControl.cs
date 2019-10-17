using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _1016
{
    class Member
    {
        public string Id { get; set; }
        public string Pw { get; set; }
        public string Name { get; set; }
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

            catch (Exception)
            {

                return false;
            }
        }

    }


}
