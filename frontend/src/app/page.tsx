import Link from "next/link";

export default function HomePage() {
  return (
    <div className="flex flex-col items-center justify-center min-h-[80vh] text-center px-4">
      <h1 className="text-5xl font-extrabold text-indigo-700 mb-4">
        🏆 Hackathon OS
      </h1>
      <p className="text-xl text-gray-600 max-w-2xl mb-8">
        The fair, bias-corrected hackathon judging & mentor platform.
        Run your hackathon with confidence — from team submission to final
        rankings.
      </p>

      <div className="grid grid-cols-1 sm:grid-cols-3 gap-6 mb-10 w-full max-w-3xl">
        <FeatureCard
          icon="⚖️"
          title="Smart Judging"
          desc="Bias-normalized scoring. Harsh or lenient judges? We fix that automatically."
        />
        <FeatureCard
          icon="👨‍🏫"
          title="Mentor Queue"
          desc="Teams request help, mentors accept. Priority queue keeps it fair and fast."
        />
        <FeatureCard
          icon="📊"
          title="Live Leaderboard"
          desc="Real-time rankings with per-criterion breakdown. Build the hype."
        />
      </div>

      <div className="flex gap-4">
        <Link
          href="/login"
          className="bg-indigo-700 text-white px-6 py-3 rounded-lg font-semibold hover:bg-indigo-800 transition"
        >
          Login
        </Link>
        <Link
          href="/register"
          className="border border-indigo-700 text-indigo-700 px-6 py-3 rounded-lg font-semibold hover:bg-indigo-50 transition"
        >
          Register
        </Link>
      </div>
    </div>
  );
}

function FeatureCard({
  icon,
  title,
  desc,
}: {
  icon: string;
  title: string;
  desc: string;
}) {
  return (
    <div className="bg-white rounded-xl shadow p-6 text-left">
      <div className="text-3xl mb-2">{icon}</div>
      <h3 className="font-bold text-lg mb-1">{title}</h3>
      <p className="text-gray-500 text-sm">{desc}</p>
    </div>
  );
}
