using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1016
{
    class Packet
    {
        #region 싱글톤 패턴
        public static Packet Instance { get; private set; }

        static Packet()
        {
            Instance = new Packet();
        }

        private Packet()
        {

        }
        #endregion

        #region 클라이언트 => 서버
        public string GetNewMember(string id, string pw, string name, int age)
        {
            string msg = null;
            msg += "NEWMEMBER@";             // 회원 가입 요청 메시지
            msg += id + "#";                  // 아이디
            msg += pw + "#";                  // 암호
            msg += name + "#";                // 이름
            msg += age;                 // 나이

            return msg;
        }
        public string GetDelMember(string id)
        {
            string msg = null;
            msg += "DELMEMBER@";
            msg += id;

            return msg;
        }

        public string GetLogin(string id, string pw)
        {
            string msg = null;
            msg += "LOGIN@";
            msg += id + "#";
            msg += pw;

            return msg;
        }

        public string GetLogOut(string id)
        {
            string msg = null;

            msg += "LOGOUT@";
            msg += id;
            return msg;
        }

        public string GetMessage(string name, string msg)
        {
            string str = null;
            str += "MESSAGE@";
            str += name + "#";
            str += msg;

            return str;
        }
        #endregion

        //#region 서버 =>클라이언트 응답
        //public string GetNewMemberAck(string id, string name, bool b)
        //{
        //    string msg = null;

        //    msg += "NEWMEMBERACK@";            // 회원 가입 요청 메시지
        //    msg += b + "#";                    //성공 실패
        //    msg += id + "#";                   // 아이디                     
        //    msg += name + "#";                 // 이름

        //    return msg;
        //}
        //#endregion


    }
}
