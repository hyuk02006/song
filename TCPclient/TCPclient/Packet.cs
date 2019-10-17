using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPclient
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

        #region 클라이언트 -> 서버

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

        public string GetLogin(string id, string pw)
        {
            string msg = null;
            msg += "LOGIN@";             // 회원 가입 요청 메시지
            msg += id + "#";                  // 아이디
            msg += pw;                  // 암호

            return msg;
        }

        public string GetDelMember(string id)
        {
            string msg = null;
            msg += "DELMEMBER@";             // 회원 가입 요청 메시지
            msg += id;                  // 아이디

            return msg;
        }

        public string GetLogOut(string id)
        {
            string msg = null;
            msg += "LOGOUT@";             // 회원 가입 요청 메시지
            msg += id;                  // 아이디

            return msg;
        }

        public string GetMessage(string id, string msg)
        {
            string msg2 = null;
            msg2 += "MESSAGE@";             // 로그아웃
            msg2 += id + "#";
            msg2 += msg;                  // 아이디

            return msg2;
        }
        #endregion

        #region 서버 -> 클라이언트 응답 패킷

        public string GetNewMemberAck(string id, string name, bool b) // 회원가입
        {
            string msg = null;
            msg += "NEWMEMBERACK@";             // 회원 가입 요청 메시지
            msg += b + "#";
            msg += id + "#";                  // 아이디
            msg += name;                // 이름

            return msg;
        }
        public string GetDelMemberAck(string id, bool b) // 회원탈퇴
        {
            string msg = null;
            msg += "DELMEMBERACK@";             // 회원 가입 요청 메시지
            msg += b + "#";
            msg += id;                  // 아이디

            return msg;

        }

        public string GetLoginAck(string id, string name)
        {
            string msg = null;
            msg += "LOGINACK@";             // 회원 가입 요청 메시지
            msg += id + "#";                  // 아이디
            msg += name;                  // 암호

            return msg;
        }

        public string GetLogOutAck(string id)
        {
            string msg = null;
            msg += "LOGOUTACK@";             // 회원 가입 요청 메시지
            msg += id;                  // 아이디

            return msg;
        }
        public string GetMessageAck(string id, string msg)
        {
            string msg2 = null;
            msg2 += "MESSAGEACK@";             // 로그아웃
            msg += id + "#";
            msg2 += msg;                  // 아이디

            return msg2;
        }


        #endregion
    }
}
