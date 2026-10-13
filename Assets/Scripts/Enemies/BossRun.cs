using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRun : StateMachineBehaviour {

	public float speed = 2.5f;
	public float attackRange = 3f;

	private Transform player;
	private Rigidbody2D rb;
	private Boss boss;

	// OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		player = GameObject.FindGameObjectWithTag("Player").transform;
		rb = animator.GetComponent<Rigidbody2D>();
		boss = animator.GetComponent<Boss>();

	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

		MinibossController miniboss = animator.GetComponent<MinibossController>();
		float pursuitRange = miniboss != null && miniboss.IsVampireGuardian ? 60f : 15f;
		if (Vector2.Distance(player.position, rb.position) <= pursuitRange) {
			boss.LookAtPlayer();

			// Di chuyển bằng velocity để Rigidbody2D xử lý collider của tường thay vì
			// dịch chuyển xuyên qua chúng bằng MovePosition trong mỗi frame animation.
			float direction = Mathf.Sign(player.position.x - rb.position.x);
			float speedMultiplier = miniboss != null ? miniboss.MovementMultiplier : 1f;
			rb.linearVelocity = new Vector2(direction * speed * speedMultiplier, rb.linearVelocity.y);
		}

		float effectiveAttackRange = attackRange * (miniboss != null ? miniboss.AttackRangeMultiplier : 1f);
		if (Vector2.Distance(player.position, rb.position) <= effectiveAttackRange) {
			animator.SetTrigger("attack");
		}
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Rigidbody2D exitingBody = animator.GetComponent<Rigidbody2D>();
		if (exitingBody != null) exitingBody.linearVelocity = new Vector2(0f, exitingBody.linearVelocity.y);
		animator.ResetTrigger("attack");
	}
}
