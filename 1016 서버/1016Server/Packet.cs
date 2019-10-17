using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1016Server
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

        #region 클라이언트 => 서버 전송
        public string GetNewMember(string id, string pw, string name, int age)
        {
            string msg = null;
            msg += "NEWMEMBER@";         // 회원 가입 요청 메시지
            msg += id + "#";              // 아이디
            msg += pw + "#";              // 암호
            msg += name + "#";            // 이름
            msg += age;                  // 나이     
            return msg;
        }

        public string GetDelMember(string id)
        {
            string msg = null;
            msg += "DELMEMBER@";         // 회원 가입 요청 메시지
            msg += id;
            return msg;
        }

        public string GetLOGIN(string id, string pw)
        {
            string msg = null;
            msg += "NEWMEMBER@";         // 회원 가입 요청 메시지
            msg += id + "#";              // 아이디
            msg += pw;                  // 나이     
            return msg;
        }

        public string GetLOGOUT(string id)
        {
            string msg = null;
            msg += "LOGOUT@";         // 회원 가입 요청 메시지
            msg += id;                  // 나이     
            return msg;
        }

        public string GetMESSAGE(string data)
        {
            string msg = null;
            msg += "MESSAGE@";         // 회원 가입 요청 메시지
            msg += data;                  // 나이     
            return msg;
        }

        #endregion


        #region 서버 => 클라이언트 응답
        public string GetNewMemberAck(string id, string name, bool b)
        {
            string msg = null;

            msg += "NEWMEMBERACK@";        // 회원 가입 요청 메시지
            msg += b + "#";
            msg += id + "#";              // 아이디
            msg += name;                  // 나이    

            return msg;
        }
        public string GetDelMemberAck(string id, bool b)
        {
            string msg = null;

            msg += "DELMEMBERACK@";        // 회원 가입 요청 메시지
            msg += b + "#";
            msg += id;                  // 나이    

            return msg;
        }
        public string GetLoginAck(string id, string pw, bool b)
        {
            string msg = null;

            msg += "LOGINACK@";        // 회원 가입 요청 메시지
            msg += b + "#";
            msg += id + "#";
            msg += pw;

            return msg;
        }
        public string GetLogoutAck(string id, bool b)
        {


            string msg = null;
            msg += "LOGOUTACK@";
            msg += b + "#";
            msg += id;
            return msg;

        }

        public string GetMessageAck(string name, string msg,bool b)
        {


            string msg2 = null;
            msg2 += "MESSAGEACK@";         // 회원 가입 요청 메시지
            msg2 += b + "#";
            msg2 += name + "#";                  // 나이  
            msg2 += msg;
            return msg2;
        }
        #endregion
    }
}
