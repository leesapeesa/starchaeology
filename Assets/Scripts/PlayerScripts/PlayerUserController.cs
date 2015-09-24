using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets._2D
{
	[RequireComponent(typeof (PlayerCharacter2D))]
	public class PlayerUserController : MonoBehaviour
	{
		private PlayerCharacter2D m_Character;
		private bool m_Jump;
		
		
		private void Awake()
		{
			m_Character = GetComponent<PlayerCharacter2D>();
		}
		
		
		private void Update()
		{
			if (!m_Jump)
			{
				// Read the jump input in Update so button presses aren't missed.
				m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
			}

		}
		
		
		private void FixedUpdate()
		{
			// Read the inputs.
			bool crouch = Input.GetKey(KeyCode.DownArrow);
			float h = CrossPlatformInputManager.GetAxis("Horizontal");
			// Pass all parameters to the character control script.
			m_Character.Move(h, crouch, m_Jump);
			m_Jump = false;
		}
	}
}
