/*
© Siemens AG, 2017-2018
Author: Dr. Martin Bischoff (martin.bischoff@siemens.com)

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain A copy of the License at
<http://www.apache.org/licenses/LICENSE-2.0>.
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using RosSharp.RosBridgeClient.MessageTypes.Geometry;

namespace RosSharp.Scripts.RosBridgeClient.RosCommuncation
{
    public class TwistPublisher : UnityPublisher<Twist>
    {

        private global::RosSharp.RosBridgeClient.MessageTypes.Geometry.Twist message;
        // private float previousRealTime;
        // private Vector3 previousPosition = Vector3.zero;
        // private Quaternion previousRotation = Quaternion.identity;
        
        public float x1, y1, z1;
        public float x2, y2, z2;
        
        protected override void Start()
        {
            base.Start();
            message = new global::RosSharp.RosBridgeClient.MessageTypes.Geometry.Twist();
        }

        private void FixedUpdate()
        { 
            message.linear.x = x1;
            message.linear.y = y1;
            message.linear.z = z1; 
            message.angular.x = x2;
            message.angular.y = y2;
            message.angular.z = z2; 
            Publish(message);
        }
    }
}